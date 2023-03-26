using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.EntityFrameworkCore;
using Xceed.Document.NET;
using Xceed.Words.NET;
using Newtonsoft.Json.Linq;

namespace HydroApp
{
    /// <summary>
    /// Логика взаимодействия для CommonPage.xaml
    /// </summary>
    public partial class CommonPage : Page
    {
        TableViewModel TableViewModel { get => (TableViewModel)Application.Current.Resources["tableVm"]; }
        CommonPageViewModel CommonPageVm { get => TableViewModel.CommonPageVm; }

        HydropressDbContext Context { get => CommonPageVm.Context; }
        public CommonPage()
        {
            InitializeComponent();

            _yearUpDown.Value = _yearUpDown.Maximum = DateTime.Now.Year;
            _monthUpDown.Value = DateTime.Now.Month;

            // generate sample data (arrays of individual DateTimes and values)
            int pointCount = 100;
            Random rand = new Random(0);
            double[] values = ScottPlot.DataGen.RandomWalk(rand, pointCount);
            DateTime[] dates = Enumerable.Range(0, pointCount)
                                          .Select(x => new DateTime(2016, 06, 27).AddDays(x))
                                          .ToArray();

            // use LINQ and DateTime.ToOADate() to convert DateTime[] to double[]
            double[] xs = dates.Select(x => x.ToOADate()).ToArray();

            // plot the double arrays using a traditional scatter plot
            var plt = mainPlot.Plot;
            plt.AddScatter(xs, values);

            // indicate the horizontal axis tick labels should display DateTime units
            plt.XAxis.DateTimeFormat(true);

            // add padding to the right to prevent long dates from flowing off the figure
            plt.YAxis2.SetSizeLimit(min: 40);

            // save the output
            plt.Title("Scatter Plot with DateTime Axis");

            mainPlot.Refresh();

            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void YearReport_Button_Click(object sender, RoutedEventArgs e)
        {
            int year = _yearUpDown.Value ?? DateTime.Today.Year; // Как-то получить год или месяц

            // загрузка

            Context.Workshops.Load();
            Context.Clients.Load();
            Context.Employees.Load();
            Context.Foremen.Load();

            Context.Constructions.Load();
            Context.Productions.Load();
            Context.Details.Load();

            Context.Batches.Load();

            Context.CommissionDetails.Load();
            Context.Commissions.Load();

            //
            var listCommissions = new List<(string, DateTime, DateTime?, decimal)>(); // название компании, 
            var listWorkshops = new List<(string, int, decimal)>();
            var listWorkers = new List<(string, int, decimal)>();


            // Заказы
            var localComs = from coms in Context.Commissions.Local
                            where (coms.ExecutionDate != null && coms.ExecutionDate.Value.Year == year) ||
                            (coms.CommissionDate.Year == year)
                            select coms;

            int allCommissions = localComs.Count();
            int allDetailForCommissions = 0;
            for (int i = 0; i < allCommissions; i++)
            {
                var el = localComs.ElementAt(i);
                listCommissions.Add((el.ClientNavigation.Name,
                el.CommissionDate,
                el.ExecutionDate,
                el.CommissionDetails.ToList().Sum(x=>x.ConstructionsAmount * x.IdConstructionNavigation.Price ?? 0)));

                allDetailForCommissions += el.CommissionDetails.ToList().Sum(x => x.ConstructionsAmount);
            }

            // Цеха

            //var workshopsLocal = Context.Workshops.Local.ToList();

            //var localBatches = from batch in Context.Batches.Local
            //                   where (batch.CompletionDate != null && batch.CompletionDate.Value.Year == year) ||
            //                   batch.RequestDate!.Value.Year == year
            //                   select batch;

            //var test = from batch in Context.Batches.Local
            //           where (batch.CompletionDate != null && batch.CompletionDate.Value.Year == year) ||
            //                   batch.RequestDate!.Value.Year == year
            //           from constr in Context.Constructions.Local
            //           where batch.Detail == constr.Id
            //           from worker in Context.Foremen.Local
            //           where batch.Foreman == worker.Id
            //           from workshop in Context.Workshops.Local
            //           where worker.Workshop == workshop.Id
            //           select (workshop.Name, batch.DetailsMadeAmount, batch.DetailsMadeAmount * constr.Price);

            listWorkshops.AddRange( 
                from batch in Context.Batches.Local
                where (batch.CompletionDate != null && batch.CompletionDate.Value.Year == year) ||
                        batch.RequestDate!.Value.Year == year
                join constr in Context.Constructions.Local on batch.Detail equals constr.Id
                join worker in Context.Foremen.Local on batch.Foreman equals worker.Id
                join workshop in Context.Workshops.Local on worker.Workshop equals workshop.Id
                select (workshop.Name, batch.DetailsMadeAmount, batch.DetailsMadeAmount * constr.Price) into selection
                group selection by selection.Name into g
                select(g.Key, g.Sum(x => x.DetailsMadeAmount), g.Sum(x => x.Item3 != null ? x.Item3!.Value : 0m)));


            // А вот в SQL это было бы:
            //
            //      SELECT SUM(DetailsMadeAmount) as sm, SUM(DetailsMadeAmount * Constructions.Price) as m, Workshops.Name
            //      FROM Batch, Constructions, Foremen INNER JOIN Workshops ON (Foremen.Workshop = Workshops.Id)
            //      WHERE Year(RequestDate) = 2023 and Batch.Detail = Constructions.Id and Batch.Foreman = Foremen.Id
            //      GROUP BY Workshops.Id, Workshops.Name



            listWorkers.AddRange( 
                from batch in Context.Batches.Local
                where (batch.CompletionDate != null && batch.CompletionDate.Value.Year == year) ||
                        batch.RequestDate!.Value.Year == year
                join constr in Context.Constructions.Local on batch.Detail equals constr.Id
                join worker in Context.Foremen.Local on batch.Foreman equals worker.Id
                select (worker.IdNavigation.FullFIO, batch.DetailsMadeAmount, batch.DetailsMadeAmount * constr.Price) into selection
                group selection by selection.FullFIO into g
                select (g.Key, g.Sum(x => x.DetailsMadeAmount), g.Sum(x => x.Item3 != null ? x.Item3!.Value : 0m)));

            //select(batch.DetailsMadeAmount, worker.IdNavigation.FullInfo, constr.Name, batch.DetailsMadeAmount * constr.Price, workshop.Name); 
            //var test2 = from obj in test
            //            where obj.item

            //localBatches.ElementAt(0).ForemanNavigation.                   

            //for (int i = 0; i <)

            var fir = listWorkers.Sum(x => x.Item2);
            var sec = listWorkshops.Sum(x => x.Item2);


            int allDetailsAmount = fir;
            
            
            decimal totalIncome = listCommissions.Sum(x=>x.Item4);


            try
            {
                
                // TO BE REMOVED

                //listCommissions = new List<(string, DateTime, DateTime?, decimal)>
                //{
                //    ("ОАО \"НефтеГазТрансБашМеталл\"", DateTime.Now.AddMonths(-6), DateTime.Now, 1425411.25M),
                //    ("ПО \"Стрела\"", DateTime.Now, null, 512651M),
                //};

                //listWorkshops = new List<(string, int, decimal)>
                //{
                //    ("Механический", 1244, 765123.12M),
                //    ("Сварочный", 1935, 12451M),
                //    ("Сборочный", 1259, 25612M),
                //};

                //listWorkers = new List<(string, int, decimal)>
                //{
                //    ("И.И. Иванов", 232, 765123.12M),
                //    ("Б.Б. Борисов", 125, 12451M),
                //    ("В.В. Вадимов", 521, 25612M),
                //};


                using (var document = DocX.Create($"Documents\\ГодовойОтчёт[{year}].docx")) // {DateTime.Now.ToString("dd-MM-yy.HH-mm-ss.fff")}
                {
                    document.SetDefaultFont(
                        new Xceed.Document.NET.Font("Times New Roman"),
                        14,
                        Color.Black);
                    var p = document.InsertParagraph();
                    p.Append("Отчёт")
                    .FontSize(16)
                    .Bold()
                    .Spacing(0)
                    .Alignment = Alignment.center;

                    p = document.InsertParagraph();
                    p.Append("\r\nДата создания: " + DateTime.Today.ToString("d"))
                        .Spacing(0)
                    .Alignment = Alignment.right;

                    p = document.InsertParagraph();
                    p.Append("Год: " + year.ToString())
                        .Spacing(0)
                    .Alignment = Alignment.right;

                    p = document.InsertParagraph();
                    p.Append("\r\nВсего создано деталей: ")
                        .Append($"{allDetailsAmount}")
                        .UnderlineStyle(UnderlineStyle.singleLine)
                        .Append(" шт.");

                    var t = document.AddTable(listCommissions.Count + 1, 4 + 1); // 4 - tuple, 1 - index
                    t.Design = TableDesign.TableGrid;
                    t.Alignment = Alignment.center;
                    t.SetWidthsPercentage(new float[] { 3f, 50f, 15.5f, 16.5f, 15f }, 1000);
                    // Заголовок
                    string[] header = { "№", "Клиент", "Дата заказа", "Дата исполнения", "На сумму (р.)" };
                    for (int i = 0; i < header.Length; i++)
                        t.Rows[0].Cells[i].Paragraphs[0].Append(header[i]);
                    t.Rows[0].Cells.ForEach(x => { x.FillColor = Color.LightGray; x.Paragraphs[0].Bold().Alignment = Alignment.center; });
                    for (int i = 0; i < listCommissions.Count; i++)
                    {
                        t.Rows[1 + i].Cells[0].Paragraphs[0].Append($"{i + 1}");
                        t.Rows[1 + i].Cells[1].Paragraphs[0].Append(listCommissions[i].Item1);
                        t.Rows[1 + i].Cells[2].Paragraphs[0].Append(listCommissions[i].Item2.ToString("d"));
                        t.Rows[1 + i].Cells[3].Paragraphs[0].Append(listCommissions[i].Item3?.ToString("d") ?? "не завершено");
                        if (listCommissions[i].Item3 == null)
                            t.Rows[1 + i].Cells[3].Paragraphs[0].Color(Color.Red);
                        t.Rows[1 + i].Cells[4].Paragraphs[0].Append(listCommissions[i].Item4.ToString("F2"));
                    }

                    p = document.InsertParagraph();
                    p.Append($"Всего выполнено заказов: ")
                        .Append($"{allCommissions}")
                        .UnderlineStyle(UnderlineStyle.singleLine)
                        .Append($" шт. (")
                        .Append($"{allDetailForCommissions}")
                        .UnderlineStyle(UnderlineStyle.singleLine)
                        .Append($" деталей), на сумму ")
                        .Append($"{totalIncome:F2}")
                        .UnderlineStyle(UnderlineStyle.singleLine)
                        .Append($" р.\r\nЗаказы:");

                    // Insert the Table after the Paragraph.
                    p.InsertTableAfterSelf(t);//Работа с таблицами

                    t = document.AddTable(listWorkshops.Count + 1, 3 + 1);
                    t.Design = TableDesign.TableGrid;
                    t.Alignment = Alignment.center;
                    t.SetWidthsPercentage(new float[] { 5f, 32f, 31f, 32f }, 1000);
                    // Заголовок
                    header = new string[] { "№", "Цех", "Создано деталей (шт.)", "На сумму (р.)" };
                    for (int i = 0; i < header.Length; i++)
                        t.Rows[0].Cells[i].Paragraphs[0].Append(header[i]);
                    t.Rows[0].Cells.ForEach(x => { x.FillColor = Color.LightGray; x.Paragraphs[0].Bold().Alignment = Alignment.center; });
                    for (int i = 0; i < listWorkshops.Count; i++)
                    {
                        t.Rows[1 + i].Cells[0].Paragraphs[0].Append($"{i + 1}");
                        t.Rows[1 + i].Cells[1].Paragraphs[0].Append(listWorkshops[i].Item1);
                        t.Rows[1 + i].Cells[2].Paragraphs[0].Append(listWorkshops[i].Item2.ToString());
                        t.Rows[1 + i].Cells[3].Paragraphs[0].Append(listWorkshops[i].Item3.ToString("F2"));
                    }

                    p = document.InsertParagraph();
                    p.Append($"\r\nПродуктивность цехов:");
                    p.InsertTableAfterSelf(t);


                    t = document.AddTable(listWorkers.Count + 1, 3 + 1);
                    t.Design = TableDesign.TableGrid;
                    t.Alignment = Alignment.center;
                    t.SetWidthsPercentage(new float[] { 5f, 32f, 31f, 32f }, 1000);
                    // Заголовок
                    header = new string[]{"№", "Работник", "Создано деталей (шт.)", "На сумму (р.)"};
                    for (int i = 0; i < header.Length; i++)
                        t.Rows[0].Cells[i].Paragraphs[0].Append(header[i]);
                    t.Rows[0].Cells.ForEach(x => { x.FillColor = Color.LightGray; x.Paragraphs[0].Bold().Alignment = Alignment.center; });
                    //t.SetColumnWidth(0, 25);
                    for (int i = 0; i < listWorkers.Count; i++)
                    {
                        t.Rows[1 + i].Cells[0].Paragraphs[0].Append($"{i + 1}");
                        t.Rows[1 + i].Cells[1].Paragraphs[0].Append(listWorkers[i].Item1);
                        t.Rows[1 + i].Cells[2].Paragraphs[0].Append(listWorkers[i].Item2.ToString());
                        t.Rows[1 + i].Cells[3].Paragraphs[0].Append(listWorkers[i].Item3.ToString("F2"));
                    }    
                    p = document.InsertParagraph();
                    p.Append($"\r\nПродуктивность сотрудников цехов:");
                    p.InsertTableAfterSelf(t);



                    // Save this document to disk.
                    document.Save();
                }
                MessageBox.Show("Отчёт успешно создан","Успех");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Отчёт не был создан:\r\n{ex.Message}", "Ошибка");
            }

        }

        private void MonthReport_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

