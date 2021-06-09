﻿using System;
using System.Collections.Generic;

using System.IO;
using System.Xml;


namespace TestSystem
{
    public partial class CTestStatistic
    {
        public class SRangeSize
        {
            public SRangeSize(int size) 
            { 
                m_size = size;
                m_range = "";
                m_nRange = 0;
            }
            public int m_size;
            public string m_range;
            public int m_nRange;

            public void AddResult(long row, long cell)
            {
                m_range += (m_range.Length > 0 ? ";" : "") + $"R{row}C{cell}";
                m_nRange++;
            }
        }
        public class SStatCollector
        {
            public string m_name { get; set; }
            public List<SRangeSize> m_aRange { get; set; }
            
            public SStatCollector(string name)
            {
                m_name = name;
                m_aRange = new List<SRangeSize>();
            }

            public string GetRange()
            {
                string result = "";
                foreach(SRangeSize r in m_aRange)
                    result += (result.Length > 0 ? ";" : "") + r.m_range;
                return result;
            }
        }

        List<SStatCollector> m_aStats;
        long m_cell;
        string m_statName;

        public CTestStatistic(string name, long cell)
        {
            m_statName = name;
            m_cell = cell;
            m_aStats = new List<SStatCollector>();
        }

        public void ReleaseOptStat(ITabler tbl)
        {
            if(m_aStats.Count > 0)
            {
                tbl.AddRow();
                tbl.AddRow();
                bool bSingle = m_aStats[0].m_aRange.Count == 1 ? true : false;
                tbl.AddCells(CTablerExcel.Styles.eStyleGreyBold, "", m_statName);
                tbl.AddCell(CTablerExcel.Styles.eStyleGreyBold, bSingle ? "Value" : "Tabbling info, avaraged by size", bSingle ? 0 : m_aStats[0].m_aRange.Count);
                
                tbl.AddRow();
                if(!bSingle)
                {
                    tbl.AddCells(CTablerExcel.Styles.eStyleGrey, "sizes", "total");
                    m_aStats[0].m_aRange.Sort((SRangeSize f, SRangeSize s) => { if(f.m_size > s.m_size) return 1; else if(f.m_size == s.m_size) return 0; else return -1; });
                    foreach(SRangeSize b in m_aStats[0].m_aRange)
                        tbl.AddCellsNumber(CTablerExcel.Styles.eStyleGrey, b.m_size);
                    tbl.AddRow();
                }
                foreach(var a in m_aStats)
                {
                    if(!bSingle)
                        a.m_aRange.Sort((SRangeSize f, SRangeSize s) => { if(f.m_size > s.m_size) return 1; else if(f.m_size == s.m_size) return 0; else return -1; });

                    string allRange = a.GetRange();
                    tbl.AddCells(CTablerExcel.Styles.eStyleGrey, a.m_name, bSingle ? "" : $"=СРЗНАЧ({allRange})");
                    foreach(SRangeSize b in a.m_aRange)
                        tbl.AddCell(CTablerExcel.Styles.eStyleGrey, $"=СРЗНАЧ({b.m_range})");
                    tbl.AddRow();
                }

                // add groups stats
                {
                    tbl.AddRow();
                    tbl.AddRow();

                    // find avg result count for group
                    int nTotal=0;
                    int excelMaxRange = 256;
                    foreach(SRangeSize a in m_aStats[0].m_aRange)
                        nTotal += a.m_nRange;
                    if(nTotal / 6 <= 200)
                        excelMaxRange = nTotal / 6;

                    foreach(var a in m_aStats)
                    {
                        tbl.AddCell(CTablerExcel.Styles.eStyleSimpleBold, a.m_name,1);
                        tbl.AddRow();
                        int nameL = 0, nameH = 0;
                        string s = "";
                        int nS = 0;
                        foreach(SRangeSize b in a.m_aRange)
                        {
                            if(nS + b.m_nRange >= excelMaxRange && nS > 0)
                            {
                                tbl.AddCells(CTablerExcel.Styles.eStyleGrey, $"от {nameL} до {nameH}", $"=СРЗНАЧ({s})");
                                tbl.AddRow();
                                nS = 0;
                                s = "";
                            }
                            if(nS == 0) nameL = b.m_size;
                            if(nS > 0) s += ";";

                            s += b.m_range;
                            nS += b.m_nRange;
                            nameH = b.m_size;
                        }
                        if(nS > 0)
                        {
                            tbl.AddCells(CTablerExcel.Styles.eStyleGrey, $"от {nameL} до {nameH}", $"=СРЗНАЧ({s})");
                            tbl.AddRow();
                            nS = 0;
                            s = "";
                        }
                    }
                    tbl.AddRow();
                    tbl.AddRow();

                }

                m_aStats.Clear();
            }
        }

        public void AddStat(string name, int size, long row)
        {
            bool bOk = false;
            foreach(SStatCollector a in m_aStats)
            {
                if(a.m_name == name)
                {
                    bool bInc = false;
                    foreach(SRangeSize b in a.m_aRange)
                    {
                        if(b.m_size == size)
                        {
                            b.AddResult(row, m_cell);
                            bInc = true;
                            break;
                        }
                    }
                    if(!bInc)
                    {
                        SRangeSize range = new SRangeSize(size);
                        range.AddResult(row, m_cell);
                        a.m_aRange.Add(range);
                    }
                    bOk = true;
                    break;
                }
            }
            if(!bOk)
            {
                SStatCollector stat = new SStatCollector(name);
                SRangeSize range = new SRangeSize(size);
                range.AddResult(row, m_cell);
                stat.m_aRange.Add(range);
                m_aStats.Add(stat);
            }
        }
    }
}