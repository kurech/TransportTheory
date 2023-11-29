﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_MM_2laba
{
    public class TransportationProblemSolver
    {
        //---------------------------------------------------------------------------
        //Функуция поиска ячеек,подлежащих циклу перераспределения (вдоль строки)
        public static double find_gor(int i_next, int j_next, int im, int jm, int n, int m, double[,] X, double[,] Y, int odd, double Xmin)
        {
            double rez = -1;
            for (int j = 0; j < m; j++)//идём вдоль строки, на которой стоим
                //ищем заполненную ячейку(кроме той,где стоим) или начальная ячейка(но уже в конце цикла:odd!=0 )
                if (((X[i_next, j] >= 0) && (j != j_next)) || ((j == jm) && (i_next == im) && (odd != 0)))
                {
                    odd++;//номер ячейки в цикле перерасчёта(начало с нуля)
                    double Xmin_old = -1;
                    if ((odd % 2) == 1)//если ячейка нечётная в цикле ( начальная- нулевая )
                    {
                        Xmin_old = Xmin;//Запоминаем значение минимальной поставки в цикле (на случай отката назад)
                        if (Xmin < 0) Xmin = X[i_next, j];//если это первая встреченная ненулевая ячейка
                        else if ((X[i_next, j] < Xmin) && (X[i_next, j] >= 0))
                        {
                            Xmin = X[i_next, j];

                        }
                    }
                    if ((j == jm) && (i_next == im) && ((odd % 2) == 0))//если замкнулся круг и цикл имеет чётное число ячеек
                    {
                        Y[im, jm] = Xmin;//Значение минимальной поставки, на величину которой будем изменять
                        return Xmin;
                    }
                    //если круг еще не замкнулся - переходим к поиску по вертикали:
                    else rez = find_ver(i_next, j, im, jm, n, m, X, Y, odd, Xmin);//рекурсивный вызов
                    if (rez >= 0)//как бы обратный ход рекурсии(в случае если круг замкнулся)
                    {
                        //для каждой ячейки цикла заполняем матрицу перерасчёта поставок:
                        if (odd % 2 == 0) Y[i_next, j] = Y[im, jm];//в чётных узлах прибавляем
                        else Y[i_next, j] = -Y[im, jm];//в нечётных узлах вычитаем
                        break;
                    }
                    else //откат назад в случае неудачи(круг не замкнулся):
                    {
                        odd--;
                        if (Xmin_old >= 0)//если мы изменяли Xmin на этой итерации
                            Xmin = Xmin_old;
                    }
                }

            return rez;//если круг замкнулся (вернулись в исходную за чётное число шагов) -
            // возвращает найденное минимальное значение поставки в нечётных ячейках цикла,
            // если круг не замкнулся, то возвращает -1.
        }
        //-----------------------------------------------------------------------------
        //Функуция поиска ячеек,подлежащих циклу перераспределения (вдоль столбца)
        public static double find_ver(int i_next, int j_next, int im, int jm, int n, int m, double[,] X, double[,] Y, int odd, double Xmin)
        {
            double rez = -1;
            int i;
            for (i = 0; i < n; i++)//идём вдоль столбца, на котором стоим
                //ищем заполненную ячейку(кроме той,где стоим) или начальная ячейка(но уже в конце цикла:odd!=0 )
                if (((X[i, j_next] >= 0)) && (i != i_next) || ((j_next == jm) && (i == im) && (odd != 0)))
                {
                    odd++;//номер ячейки в цикле перерасчёта(начало с нуля)
                    double Xmin_old = -1;
                    if ((odd % 2) == 1)//если ячейка нечётная в цикле ( начальная- нулевая )
                    {
                        Xmin_old = Xmin;//Запоминаем значение минимальной поставки в цикле (на случай отката назад)
                        if (Xmin < 0) Xmin = X[i, j_next];//если это первая встреченная ненулевая ячейка
                        else
                            if ((X[i, j_next] < Xmin) && (X[i, j_next] >= 0))
                            Xmin = X[i, j_next];
                    }
                    if ((i == im) && (j_next == jm) && ((odd % 2) == 0))//если замкнулся круг и цикл имеет чётное число ячеек
                    {
                        Y[im, jm] = Xmin;//Значение минимальной поставки, на величину которой будем изменять
                        return Xmin;
                    }
                    //если круг еще не замкнулся - переходим к поиску по горизонтали:
                    else
                        rez = find_gor(i, j_next, im, jm, n, m, X, Y, odd, Xmin);//- рекурсивный вызов
                    if (rez >= 0)//как бы обратный ход (в случае если круг замкнулся)
                    {
                        //для каждой ячейки цикла заполняем матрицу перерасчёта поставок:
                        if (odd % 2 == 0) Y[i, j_next] = Y[im, jm];//эти прибавляются
                        else Y[i, j_next] = -Y[im, jm];//эти вычитаются
                        break;
                    }
                    else //откат назад в случае неудачи (круг не замкнулся):
                    {
                        odd--;
                        if (Xmin_old >= 0)//если мы изменяли Xmin на этой итерации
                            Xmin = Xmin_old;
                    }
                }

            return rez;
        }
    }

}