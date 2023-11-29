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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_MM_2laba
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Конструктор окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            nxtButtin.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Переход на ввод значений для задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nxtButtin_Click(object sender, RoutedEventArgs e)
        {
            if(stolbItem.SelectedItem != null && strItem.SelectedItem != null)
            {
                int columns = int.Parse((stolbItem.SelectedItem as ComboBoxItem)?.Content.ToString());
                int rows = int.Parse((strItem.SelectedItem as ComboBoxItem)?.Content.ToString());

                for (int i = 1; i < rows + 2; i++)
                {
                    StackPanel rowPanel = new StackPanel { Orientation = Orientation.Horizontal };

                    for (int j = 1; j < columns + 2; j++)
                    {
                        TextBox textBox = new TextBox { Width = 100, Height = 30, Margin = new Thickness(5), Name = "txtNumber" + i.ToString() + "_" + j.ToString(), Text = i.ToString() + "_" + j.ToString() };
                        rowPanel.Children.Add(textBox);

                        if (i == rows + 1 || j == columns + 1)
                        {
                            textBox.Background = Brushes.LightSlateGray;
                        }

                        if (j == columns + 2 - 1 && i == rows + 2 - 1)
                        {
                            rowPanel.Children.Remove(textBox);
                        }
                    }

                    matrixStack.Children.Add(rowPanel);
                }

                topStck.IsEnabled = false;
                findResultButtin.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Выберите количество строк и столбцов");
            }
        }

        /// <summary>
        /// Получение значений из TextBox
        /// </summary>
        /// <returns></returns>
        private Tuple<double[,], bool> GetValuesFromTextBoxes()
        {
            int rows = int.Parse((strItem.SelectedItem as ComboBoxItem)?.Content.ToString());
            int cols = int.Parse((stolbItem.SelectedItem as ComboBoxItem)?.Content.ToString());

            double[,] values = new double[rows, cols];
            bool flag = false;

            for (int i = 1; i <= rows; i++)
            {
                for (int j = 1; j <= cols; j++)
                {
                    TextBox textBox = FindTextBoxByName("txtNumber" + (i).ToString() + "_" + (j).ToString()) as TextBox;

                    if (textBox != null && int.TryParse(textBox.Text, out int value))
                    {
                        values[i - 1, j - 1] = value;
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
            return Tuple.Create(values, flag);
        }


        /// <summary>
        /// Получение запасов из TextBox
        /// </summary>
        /// <returns></returns>
        private double[] GetSupplyFromTextBoxes()
        {
            int rows = int.Parse((strItem.SelectedItem as ComboBoxItem)?.Content.ToString());
            int cols = int.Parse((stolbItem.SelectedItem as ComboBoxItem)?.Content.ToString());

            double[] supply = new double[rows];

            for (int i = 1; i <= rows; i++)
            {
                TextBox textBox = FindTextBoxByName("txtNumber" + i.ToString() + "_" + (cols+1).ToString()) as TextBox;

                if (textBox != null && int.TryParse(textBox.Text, out int value))
                {
                    supply[i - 1] = value;
                }
            }
            return supply;
        }

        /// <summary>
        /// Получение поставок из TextBox
        /// </summary>
        /// <returns></returns>
        private double[] GetDemandFromTextBoxes()
        {
            int rows = int.Parse((strItem.SelectedItem as ComboBoxItem)?.Content.ToString());
            int cols = int.Parse((stolbItem.SelectedItem as ComboBoxItem)?.Content.ToString());

            double[] demand = new double[cols];

            for (int i = 1; i <= cols; i++)
            {
                TextBox textBox = FindTextBoxByName("txtNumber" + (rows + 1).ToString() + "_" + i.ToString()) as TextBox;

                if (textBox != null && int.TryParse(textBox.Text, out int value))
                {
                    demand[i - 1] = value;
                }
            }
            return demand;
        }

        /// <summary>
        /// Метод для поиска TextBox по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private TextBox FindTextBoxByName(string name)
        {
            foreach (var child in matrixStack.Children)
            {
                if (child is StackPanel stackPanel)
                {
                    foreach (var stackPanelChild in stackPanel.Children)
                    {
                        if (stackPanelChild is TextBox textBox && textBox.Name == name)
                        {
                            return textBox;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Решение задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findResultButtin_Click(object sender, RoutedEventArgs e)
        {
            double[,] costs = GetValuesFromTextBoxes().Item1;

            double[] supply = GetSupplyFromTextBoxes();
            double[] demand = GetDemandFromTextBoxes();

            double sum_supply = 0;
            for (int i = 0; i < supply.Length; i++)
            {
                sum_supply += supply[i];
            }

            double sum_demand = 0;
            for (int i = 0; i < demand.Length; i++)
            {
                sum_demand += demand[i];
            }

            if (GetValuesFromTextBoxes().Item2 == false)
            {
                if (sum_supply == sum_demand)
                {
                    double[,] solution = calculate(costs, supply, demand).Item1;

                    MessageBox.Show(message.ToString(), "Ход решения задачи:");
                    message.Clear();

                    double F = calculate(costs, supply, demand).Item2;
                    message.Clear();

                    ShowArrayInMessageBox(solution, F);
                }
                else
                {
                    MessageBox.Show($"Сумма поставок не соответсвует сумме запасов! {sum_supply} != {sum_demand}");
                }
            }
            else
            {
                MessageBox.Show("Неправильно введены значения!");
            }
        }


        /// <summary>
        /// Отображение результата
        /// </summary>
        /// <param name="array"></param>
        /// <param name="F"></param>
        private void ShowArrayInMessageBox(double[,] array, double F)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);

            StringBuilder message1 = new StringBuilder();

            for (int i = 0; i < rows-1; i++)
            {
                for (int j = 0; j < cols-1; j++)
                {
                    message1.Append(array[i, j].ToString() + "\t");
                }
                message1.AppendLine(); // Переход на новую строку после каждой строки матрицы
            }

            message1.AppendLine($"Fmin = {F}");

            MessageBox.Show(message1.ToString(), "Результат решения");
        }


        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.Shutdown();
        }


        /// <summary>
        /// Обновление окна (переход к новой задаче)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateButtin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            Hide();
        }


        /// <summary>
        /// Загрузка окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            findResultButtin.IsEnabled = false;
        }
        public static StringBuilder message = new StringBuilder();
        public static Tuple<double[,], double> calculate(double[,] A, double[] zapasu, double[] potrebnosti)
        {
            int N, M;        //Размерность задачи
            double[] a;                   //адрес массива запасов поставщиков
            double[] b;                        //адрес массива потребностей потребителей
            double[,] C;                  //адресмассива(двумерного) стоимости перевозки
            double[,] X;                 //адрес массива(двумерного) плана доставки
            double[] u;                  //адрес массива потенциалов поставщиков
            double[] v;              //адрес массива потенциалов потребителей
            double[,] D;                 //адрес массива(двумерного) оценок свободных ячеек таблицы
            bool stop;              //признак достижения оптимального плана
            bool[,] T;              //массив будет хранить коодинаты ячеек, в которые уже вписывались
            bool ok = true;           //нулевые поставки при попытках устранить вырожденность плана


            int n = A.GetLength(0);  //кол-во строк таблицы стоимости
            int m = A.GetLength(1);  //кол-во столбцов таблицы стоимости

            //-------------------Проверка на сбалансированность
            double Sa = 0.0;
            double Sb = 0.0;


            for (int i = 0; i < n; i++)          //находим суммарные запасы
                Sa = Sa + zapasu[i];

            for (int i = 0; i < m; i++)          //находим суммарную потребность
                Sb = Sb + potrebnosti[i];

            //----------------------Инициализация динамических массивов:

            //запоминаем размерность в глобальных переменных:
            N = n;              //кол-во строк (поставщиков)
            M = m;              //кол-во столбцов (потребителей)

            //выделение памяти под динамические массивы и их заполнение:
            a = zapasu;        //массив для Запасов

            b = potrebnosti;    //Массив для Потребностей

            //Двумерный массив для Стоимости:
            C = A;

            //Двумерный массив для Доставки:
            X = new double[N + 1, M + 1];//выделяем память под массив адресов начала строк
            /*
             В последней строке(столбце) массива Х будем записывать
             сумму заполненных клеток в соответствующем столбце(строке)
            */
            for (int i = 0; i < N + 1; i++)
                for (int j = 0; j < M + 1; j++)
                {
                    X[i, j] = -1;           //вначале все клетки не заполнены
                    if (i == N)
                        X[i, j] = 0;        //сумма заполненных клеток в j-м столбце
                    if (j == M)
                        X[i, j] = 0;        //сумма заполненных клеток в i-й строке
                }


            //-----------------Метод минимального элемента:

            double Sij = 0;
            do
            {
                int im = 0;
                int jm = 0;
                double Cmin = -1;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                        if (X[N, j] != b[j])//если не исчерпана Потребность Bj
                            if (X[i, M] != a[i])//если не исчерпан Запас Аі
                                if (X[i, j] < 0)//если клетка ещё не заполнена
                                {
                                    if (Cmin == -1)//если это первая подходящая ячейка
                                    {
                                        Cmin = C[i, j];
                                        im = i;
                                        jm = j;
                                    }
                                    else //если это не первая подходящая ячейка
                                        if (C[i, j] <= Cmin)//если в ячейке меньше,чем уже найдено
                                    {
                                        Cmin = C[i, j];
                                        im = i;
                                        jm = j;
                                    }
                                }

                X[im, jm] = Math.Min(a[im] - X[im, M], b[jm] - X[N, jm]);//выбираем поставку
                X[N, jm] = X[N, jm] + X[im, jm];//добавляем поставку jm-му потребителю
                X[im, M] = X[im, M] + X[im, jm];//добавляем поставку im-му поставщику
                Sij = Sij + X[im, jm]; //Подсчёт суммы добавленых поставок

            } while (Sij < Math.Max(Sa, Sb));//условие продолжения

            int L = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                    if (X[i, j] >= 0) L++;          //подсчёт заполненных ячеек
            int d = M + N - 1 - L;                  //если d>0,то задача - вырожденная,придётся добавлять d нулевых поставок
            int d1 = d;                             //запоминаем значение d

            /*message.Append("Начальный опорный план:");
            message.AppendLine();

            double F = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                {
                    message.Append(X[i, j]);
                    message.Append("\t");
                    if (X[i, j] > 0)
                        F = F + X[i, j] * C[i, j];
                    if (j == M - 1)
                        message.AppendLine();
                }

            message.Append("----------------------");
            message.AppendLine();
            message.Append("F = " + F);*/

            //--------------------------Метод потенциалов

            T = new bool[N, M];

            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                    T[i, j] = false;

            do
            {//цикл поиска оптимального решения
                stop = true;//признак оптимальности плана(после проверки может стать false)
                u = new double[N];//выделение массивов под значения потециалов
                v = new double[M];
                bool[] ub = new bool[N];//вспомогательные массвы
                bool[] vb = new bool[M];
                for (int i = 0; i < N; i++)
                    ub[i] = false;
                for (int i = 0; i < M; i++)
                    vb[i] = false;


                //цикл расчёта потенциалов (несколько избыточен):
                u[0] = 0;              //значение одного потенциала выбираем произвольно
                ub[0] = true;
                int count = 1;
                int tmp = 0;
                do
                {
                    for (int i = 0; i < N; i++)
                        if (ub[i] == true)
                            for (int j = 0; j < M; j++)
                                if (X[i, j] >= 0)
                                    if (vb[j] == false)
                                    {
                                        v[j] = C[i, j] - u[i];
                                        vb[j] = true;
                                        count++;
                                    }
                    for (int j = 0; j < M; j++)
                        if (vb[j] == true)
                            for (int i = 0; i < N; i++)
                                if (X[i, j] >= 0)
                                    if (ub[i] == false)
                                    {
                                        u[i] = C[i, j] - v[j];
                                        ub[i] = true;
                                        count++;
                                    }
                    tmp++;
                } while ((count < (M + N - d * 2)) && (tmp < M * N));
                message.AppendLine();
                message.Append("----------------------");
                message.AppendLine();

                //цикл добавления нулевых поставок(в случае вырожденности):
                bool t = false;

                if ((d > 0) || ok == false) t = true;//цикл начинается, если d>0
                while (t)//цикл продолжается до тех пор, пока все потенциалы не будут найдены
                {
                    for (int i = 0; (i < N); i++)//просматриваем потенциалы поставщиков
                        if (ub[i] == false)//если среди них не заполненный потенциал
                            for (int j = 0; (j < M); j++)
                                if (vb[j] == true)
                                {
                                    if (d > 0)
                                        if (T[i, j] == false)//если раньше не пытались использовать
                                        {
                                            X[i, j] = 0;        //добавляем нулевую поставку
                                            d--;                //уменьшаем кол-во требуемых добавлений нулевых поставок
                                            T[i, j] = true;     //отмечаем, что эту ячейку уже использовали
                                        }
                                    if (X[i, j] >= 0)
                                    {
                                        u[i] = C[i, j] - v[j];//дозаполняем потенциалы
                                        ub[i] = true;
                                    }
                                }
                    for (int j = 0; (j < M); j++)//просматриваем потенциалы потребителей
                        if (vb[j] == false)//если среди них не заполненный потенциал
                            for (int i = 0; (i < N); i++)
                                if (ub[i] == true)
                                {
                                    if (d > 0)
                                        if (T[i, j] == false)//если раньше не пытались использовать
                                        {
                                            X[i, j] = 0;//добавляем нулевую поставку
                                            d--;//уменьшаем кол-во требуемых добавлений нулевых поставок
                                            T[i, j] = true;//отмечаем, что эту ячейку уже использовали
                                        }
                                    if (X[i, j] >= 0)
                                    {
                                        v[j] = C[i, j] - u[i];//дозаполняем потенциалы
                                        vb[j] = true;
                                    }
                                }
                    t = false; //проверяем, все ли потенциалы найдены
                    for (int i = 0; i < N; i++)
                        if (ub[i] == false) t = true;
                    for (int j = 0; j < M; j++)
                        if (vb[j] == false) t = true;

                    if (t == false)
                    {
                        message.Append("Опорный план после устранения вырожденности:");
                        message.AppendLine();
                        //Console.WriteLine("Опорный план после устранения вырожденности:");
                        //Console.WriteLine("");
                        for (int i = 0; i < N; i++)
                            for (int j = 0; j < M; j++)
                            {
                                message.Append(X[i, j]);
                                //Console.Write("{0,4}", X[i, j]);
                                if (j == M - 1)
                                    message.AppendLine();
                                //Console.WriteLine(Environment.NewLine);
                            }
                        message.AppendLine();
                        //Console.WriteLine("--------");
                    }

                }
                //-----------

                message.Append("Потенциалы:");
                message.AppendLine();
                message.Append("u: ");
                message.AppendLine();
                for (int i = 0; i < N; i++)
                {
                    message.Append(u[i]);
                    message.Append("\t");
                }
                message.AppendLine();
                message.Append("v: ");
                message.AppendLine();
                for (int i = 0; i < M; i++)
                {
                    message.Append(v[i]);
                    message.Append("\t");
                }

                message.AppendLine();
                message.Append("----------------------");

                //----------------------

                D = new double[N, M];//выделяем память под массив оценок свободных ячеек

                //int Dmin=0;
                //int im=-1;
                //int jm=-1;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                    {
                        if (X[i, j] >= 0)//если ячейка не свободна
                            D[i, j] = 88888;//Заполняем любыми положительными числами
                        else  //если ячейка свободна
                            D[i, j] = C[i, j] - u[i] - v[j];//находим оценку

                        if (D[i, j] < 0)
                        {
                            stop = false;//признак того, что план не оптимальный
                        }

                    }
                //
                message.AppendLine();
                message.Append("Матрица оценок свободных ячеек (если ячейка занята - ставим 88888)");
                message.AppendLine();
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                    {
                        message.Append(D[i, j]);
                        message.Append("\t");
                        if (j == M - 1)
                            message.AppendLine();
                    }
                message.Append("----------------------");
                message.AppendLine();
                //
                if (stop == false)//если план не оптимальный
                {
                    double[,] Y = new double[N, M];//массив для хранения цикла перераспределения поставок

                    double find1, find2;//величина перераспределения поставки для цикла
                    double best1 = 0;//наилучшая оценка улучшения среди всех допустимых перераспределений
                    double best2 = 0;
                    int ib1 = -1;
                    int jb1 = -1;
                    int ib2 = -1;
                    int jb2 = -1;
                    //Ищем наилучший цикл перераспределения поставок:
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                            if (D[i, j] < 0)//Идём по ВСЕМ ячейкам с отрицательной оценкой
                            {  //и ищем допустимые циклы перераспределения ДЛЯ КАЖДОЙ такой ячейки
                                //Обнуляем матрицу Y:
                                for (int i1 = 0; i1 < N; i1++)
                                    for (int j1 = 0; j1 < M; j1++)
                                        Y[i1, j1] = 0;
                                //Ищем цикл для ячейки с оценкой D[i,j]:
                                find1 = TransportationProblemSolver.find_gor(i, j, i, j, N, M, X, Y, 0, -1);   //Начинаем идти по горизонтали

                                //Обнуляем матрицу Y:
                                for (int i1 = 0; i1 < N; i1++)
                                    for (int j1 = 0; j1 < M; j1++)
                                        Y[i, j] = 0;

                                find2 = TransportationProblemSolver.find_ver(i, j, i, j, N, M, X, Y, 0, -1);//Начинаем по вертикали

                                if (find1 > 0)
                                    if (best1 > D[i, j] * find1)
                                    {
                                        best1 = D[i, j] * find1;     //наилучшая оценка
                                        ib1 = i;                   //запомминаем координаты ячейки
                                        jb1 = j;                   //цикл из которой даёт наибольшее улучшение
                                    }
                                if (find2 > 0)
                                    if (best2 > D[i, j] * find2)
                                    {
                                        best2 = D[i, j] * find2; //наилучшая оценка
                                        ib2 = i;              //запомминаем координаты ячейки
                                        jb2 = j;              //цикл из которой даёт наибольшее улучшение
                                    }
                            }
                    if ((best1 == 0) && (best2 == 0))
                    {
                        //stop=true;
                        //ShowMessage("Цикл перераспределения поставок не найден");
                        ok = false;
                        d = d1;//откат назад
                        for (int i = 0; i < N; i++)
                            for (int j = 0; j < M; j++)
                                if (X[i, j] == 0) X[i, j] = -1;
                        continue;
                    }
                    else
                    {   //Обнуляем матрицу Y:
                        for (int i = 0; i < N; i++)
                            for (int j = 0; j < M; j++)
                                Y[i, j] = 0;
                        //возвращаемся к вычислению цикла с наилучшим перераспределением:
                        int ib, jb;
                        if (best1 < best2)
                        {
                            TransportationProblemSolver.find_gor(ib1, jb1, ib1, jb1, N, M, X, Y, 0, -1);
                            ib = ib1;
                            jb = jb1;
                        }
                        else
                        {
                            TransportationProblemSolver.find_ver(ib2, jb2, ib2, jb2, N, M, X, Y, 0, -1);
                            ib = ib2;
                            jb = jb2;
                        }
                        for (int i = 0; i < N; i++)
                        {
                            for (int j = 0; j < M; j++)
                            {
                                if ((X[i, j] == 0) && (Y[i, j] < 0))
                                {
                                    stop = true;
                                    ok = false;
                                    d = d1;//откат назад
                                    message.Append("Попытка отрицательной поставки!");
                                    //ShowMessage("Попытка отрицательной поставки!");
                                    //break;
                                }
                                X[i, j] = X[i, j] + Y[i, j];//перераспределяем поставки
                                if ((i == ib) && (j == jb)) X[i, j] = X[i, j] + 1;//добавляем 1 (т.к. до этого было -1 )
                                if ((Y[i, j] <= 0) && (X[i, j] == 0)) X[i, j] = -1;//если ячейка обнулилась, то выбрасываем её из рассмотрения
                            }
                            //if(stop)break;
                        }
                    }
                    //
                    message.AppendLine();
                    message.Append("Матрица цикла перерасчёта:");
                    message.AppendLine();
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                        {
                            message.Append(Y[i, j]);
                            //Console.Write("{0,4}", Y[i, j]);
                            if (j == M - 1)
                                message.AppendLine();
                            //Console.WriteLine("");
                        }
                    message.Append("--------");
                    message.AppendLine();

                    message.Append("Новый план:");
                    message.AppendLine();
                    double F1 = 0;
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                        {
                            message.Append(X[i, j]);
                            if (X[i, j] > 0)
                                F1 = F1 + X[i, j] * C[i, j];
                            if (j == M - 1)
                                message.AppendLine();
                        }
                    message.Append($"F = {F1}");
                    message.AppendLine();
                    message.Append("--------");
                    message.AppendLine();

                    // ShowMessage("Для продолжения нажмите \"ОК\" ");
                    //

                    ok = true;
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                            T[i, j] = false;

                    //проверка на вырожденность: (?)
                    L = 0;
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < M; j++)
                            if (X[i, j] >= 0) L++;//подсчёт заполненных ячеек
                    d = M + N - 1 - L;//если d>0,то задача - вырожденная
                    d1 = d;
                    if (d > 0) ok = false;
                }
            } while (stop == false);

            message.Append("Оптимальный план:");
            message.AppendLine();

            double Fmin = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                {
                    message.Append(X[i, j]);
                    message.Append("\t");
                    if (X[i, j] > 0)
                        Fmin = Fmin + X[i, j] * C[i, j];
                    if (j == M - 1)
                        message.AppendLine();
                }
            message.Append("----------------------");
            message.AppendLine();
            message.Append($"Fmin = {Fmin}");

            

            return Tuple.Create(X, Fmin);
        }
    }
}
