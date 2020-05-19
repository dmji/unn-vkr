﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QAP
{
	public partial class info
	{
		public individ test_generator(int sizeQAP, int omeg, int z, individ p = null)
		{
            static int[] rs_count(int sizeQAP)
            {
                List<int> rs = new List<int>();
                for (int i = 2; i < sizeQAP; i++)
                    if (sizeQAP % i == 0) rs.Add(i);
                while (rs.Count > 2) { rs.RemoveAt(0); rs.RemoveAt(rs.Count - 1); }
                if (rs.Count == 0) { rs.Add(1); rs.Add(sizeQAP); }
                else if (rs.Count == 1) rs.Add(rs[0]);
                return rs.ToArray();
            }

            if (p == null)
                p = new individ(sizeQAP);

            List<List<int>> D = new List<List<int>>();
            int[] rs = rs_count(sizeQAP);
            //D countig
            for (int y = 0; y < rs[1]; y++)
                for (int x = 0; x < rs[0]; x++)
                {
                    D.Add(new List<int>());
                    for (int yP = 0; yP < rs[1]; yP++)
                        for (int xP = 0; xP < rs[0]; xP++)
                            D[D.Count - 1].Add(Math.Abs(x - xP) + Math.Abs(y - yP));
                }
            // omeg & g declare
            List<List<int>> omegar = new List<List<int>>(sizeQAP);
            for (int i = 0; i < sizeQAP; i++)
            {
                omegar.Add(new List<int>(sizeQAP));
                for (int j = 0; j < sizeQAP; j++)
                    omegar[i].Add(omeg);
            }
            List<List<int>> g = new List<List<int>>(sizeQAP);
            for (int i = 0; i < sizeQAP; i++)
            {
                g.Add(new List<int>(sizeQAP));
                for (int j = 0; j < sizeQAP; j++)
                    g[i].Add(2 - D[i][j]);
            }
            int [,] lm = new int[sizeQAP, sizeQAP];
            do
            {
                int l = -1, m = -1;
                for (int i = 0; i < sizeQAP; i++)
                    for (int j = 0; j < sizeQAP; j++)
                        if (g[i][j] <= 0 && lm[i, j] == 0)
                        {
                            if (l == -1)
                            {
                                l = i; 
                                m = j;
                            }
                            else
                            {
                                if (D[l][m] < D[i][j] && lm[i, j] == 0)
                                {
                                    l = i; 
                                    m = j;
                                }
                            }
                        }
                if (l == -1 && m == -1)
                    break;
                lm[l, m] = 1;
                int k = 0, delt = new Random().Next(z);
                do
                {
                    k = new Random().Next(sizeQAP);
                } while (Math.Abs(D[l][k] - D[m][k]) > 1);
                omegar[l][m] = delt;
                omegar[l][k] = omegar[l][k] + (omeg - delt);
                omegar[m][k] = omegar[m][k] + (omeg - delt);
                g[l][m] = 1;
                g[l][k] = 1;
                g[m][k] = 1;
            } while (true);

            List<List<int>> F = new List<List<int>>(sizeQAP);
            for (int i = 0; i < sizeQAP; i++)
            {
                F.Add(new List<int>(sizeQAP));
                for (int j = 0; j < sizeQAP; j++)
                    F[i].Add(0);
            }

            for (int i = 0; i < sizeQAP; i++)
                for (int j = 0; j < sizeQAP; j++)
                    F[i][j] = omegar[p[i]][p[j]];

            base_init(sizeQAP);
            problem_size = sizeQAP;
            for (int i = 0; i < sizeQAP; i++)
            {
                for (int j = 0; j < sizeQAP; j++)
                {
                    stream[i,j] = D[i][j];
                    price[i,j] = F[i][j];
                } 
            }
            return p;
        }

        public void export_txt(individ p=null, int omeg=-1, int z=-1)
        {
            string buf = problem_size.ToString() + "\n";
            for (int i = 0; i < problem_size; i++)
                for (int j = 0; j < problem_size; j++)
                {
                    if (j == problem_size - 1)
                        buf = buf + price[i, j] + "\n";
                    else
                        buf = buf + price[i, j] + " ";
                }
            buf = buf + '\n';
            for (int i = 0; i < problem_size; i++)
                for (int j = 0; j < problem_size; j++)
                {
                    if (j == problem_size - 1)
                        buf = buf + stream[i, j] + "\n";
                    else
                        buf = buf + stream[i, j] + " ";
                }
            buf = buf + '\n';
            for (int i = 0; i < problem_size; i++)
                for (int j = 0; j < problem_size; j++)
                {
                    if (j == problem_size - 1)
                        buf = buf + position_cost[i, j] + "\n";
                    else
                        buf = buf + position_cost[i, j] + " ";
                }
            if (p != null)
            {
                buf = buf + "\np={";
                for (int i = 0; i < problem_size; i++)
                    buf = buf + p[i] + ", ";
                buf = buf + "}";
            }
            StreamWriter file;
            if (omeg==-1)
                 file = new StreamWriter("ex_"+problem_size+".txt");
            else
                file = new StreamWriter("ex_"+problem_size+" "+omeg+" "+z+".txt");
            file.WriteLine(buf);
            file.Close();
        }
	}
}
