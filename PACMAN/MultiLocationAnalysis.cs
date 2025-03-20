using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using RandomNumberGenerator;

namespace PACMAN
{
    public class MultiLocationAnalysis
    {

        private Dictionary<string, PollutionModel> PollutionData;

        public MultiLocationAnalysis()
        {

        }

        public CorrosionData BeginAnalysis(List<AnalysisTask> _tasks, bool finalAnalysis, string tailNumber, List<sensorData> sensordata, Dictionary<string, PollutionModel> pollutionData, ref CorrosionData cd)
        {
            SimpleRNG.SetSeedFromSystemTime();
            int numTasks = _tasks.Count();
            cd.cutoffValuesDroplets = new int[numTasks];
            cd.AircraftTail = tailNumber;

            List<List<enviroP>> enviroP_List = new List<List<enviroP>>();
            //List<sensorData> sensorData = new List<sensorData>();

            PollutionData = pollutionData;

            AnalysisTask _task = new AnalysisTask();
            _task.WashFreq = 0;
            _task.DetergentWash = false;
            _task.MetDataType = MetDataTypes.Meteorological;
            _task.CrystalSize = 6;

            SensorCalib(_task, ref enviroP_List, sensordata, ref cd, numTasks);
            double[,,] PitString = coelesence(ref enviroP_List, _task);
            pit(ref enviroP_List, _task, PitString, ref cd);

            return cd;
        }

        public int BeginMultiTailAnalysis(DateTime StartDate, DateTime EndDate)
        {
            int value = 0;

            int numberOfDays = (EndDate - StartDate).Days;

            double largestPit = 5.0;

            if (numberOfDays <= 356)
            {
                largestPit = 50;
            }
            else if (numberOfDays <= 730)
            {
                largestPit = 100;
            }
            else if (numberOfDays <= 1095)
            {
                largestPit = 150;
            }
            else if (numberOfDays <= 1460)
            {
                largestPit = 200;
            }
            else
            {
                largestPit = 250;
            }

            value = (int)((largestPit / 250) * 100);

            if (value > 100)
            {
                value = 100;
            }

            return value;
        }


        public CorrosionData SensorCalib(AnalysisTask _task, ref List<List<enviroP>> enviroP_List, List<sensorData> sensorData, ref CorrosionData cd, int numTasks)
        {
            int dropletArrayIndex = 0;
            List<double> tempx = new List<double>();
            List<double> rhx = new List<double>();
            List<double> precx = new List<double>();
            List<double> towx = new List<double>();
            List<double> surfacex = new List<double>();
            List<double> dropsx = new List<double>();
            List<DateTime> tempy = new List<DateTime>();
            List<int> tempy2 = new List<int>();
            DateTime savedMinDate = DateTime.MinValue;
            double[,] Pollutant;

            Random rng = new Random();
            int p = 0;
            int q = 0;
            int t = 0;
            List<double> TOW = new List<double>();
            List<double> temp = new List<double>();
            List<nDrops> nDrops = new List<nDrops>();


            double PredA = 0, PredB1 = 0, PredB2 = 0, PredB3 = 0, NPredA = 0, NPredB1 = 0, NPredB2 = 0, Clean = 0;
            double TotDrop = 0, NCCon = 0, MCCon = 0, NSCon = 0, NNCon = 0, NHSCon = 0, TotCon = 0, NCVol = 0, MCVol = 0, NSVol = 0, NNVol = 0, NHSVol = 0, TotVol = 0, AvgRad = 0, CCCon = 0, KCCon = 0, MSCon = 0, KSCon = 0, CCVol = 0, KCVol = 0, MSVol = 0, KSVol = 0;
            double TotArea = 0, NCMass = 0, MCMass = 0, NSMass = 0, NNMass = 0, NHSMass = 0, TotMass = 0, CaSul = 0, CrysDens = 0, DropRad = 0, CrysMass = 0, DropArea = 0, DropVol = 0, CCMass = 0, KCMass = 0, MSMass = 0, KSMass = 0;
            int crysize = 0;

            double NdropsR = 0, NdropsT = 0, CumRH = 0, CumRHT = 0, MinConc = 0, MaxConc = 0, SaltType = 0, Saltconc = 0, Oxyconc = 0, satconc = 0, Lim2024 = 0, Lim7050 = 0, DO2 = 0, OxyconcA = 0, CumTime;
            int z = 0;

            //TEMP OBJECT
            List<enviroP> tempList = new List<enviroP>();
            enviroP tempEnviroP = new enviroP();
            tempList.Add(tempEnviroP);

            //ADD THE FIRST ARRAY INDEX WHICH WILL CONTAIN THE PERIODS
            List<int> period_list = new List<int>();
            //insert the 0th element, make it 0.
            period_list.Add(0);
            enviroP_List.Add(tempList);



            if (_task.MetDataType == MetDataTypes.Meteorological)
            {

                //Pollutant Levels
                double Day = 0;
                double Days = 0;

                int size = sensorData.Count;

                Pollutant = new double[size + 1, 3];
                double[] Chloride = new double[size + 1];
                Pollutant[0, 0] = 0;
                TOW.Add(0);

                for (int w = 1; w < size; w++)
                {

                    Pollutant[w, 0] = Pollutant[w - 1, 0] + (getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "Deposit") * Convert.ToDouble(sensorData[w].col3)) / (3600d * 24d);
                    Day = Day + (sensorData[w].col3) / (3600d * 24d);
                    Days = Days + (sensorData[w].col3) / (3600d * 24d);
                    Pollutant[w, 2] = Days;
                    if (sensorData[w].col4 >= 99.5)
                        sensorData[w].col4 = 99.5;

                    if (sensorData[w].col5 == 0)
                        Pollutant[w, 1] = 0;
                    else
                        Pollutant[w, 1] = sensorData[w].col5;

                    if (sensorData[w].col5 > 0)
                        Pollutant[w, 1] = Pollutant[w - 1, 1] + sensorData[w].col5;

                    if (Pollutant[w, 1] > 0.055)
                        Pollutant[w, 0] = 0.14 * getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "RainConc");
                    else
                        Pollutant[w, 0] = Pollutant[w, 0] + (sensorData[w].col5 * 25.4) * getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "RainConc");

                    if (_task.WashFreq > 0)
                        if (Day >= _task.WashFreq)
                        {
                            Day = Day - _task.WashFreq;
                            if (_task.DetergentWash)
                                Pollutant[w, 0] = 10d;
                            else
                                Pollutant[w, 0] = 20d;
                        }

                    //number of crystals
                    NCMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "NaCl") * Pollutant[w, 0] / 100;
                    MCMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "MgC12") * Pollutant[w, 0] / 100;
                    NSMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "Na2SO4") * Pollutant[w, 0] / 100;
                    CCMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "CaC12") * Pollutant[w, 0] / 100;
                    KCMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "KCI") * Pollutant[w, 0] / 100;
                    NNMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "AmmNit") * Pollutant[w, 0] / 100;
                    NHSMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "AmmSul") * Pollutant[w, 0] / 100;
                    MSMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "MgSO4") * Pollutant[w, 0] / 100;
                    KSMass = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "K2SO4") * Pollutant[w, 0] / 100;

                    CaSul = 100 - getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "Total") + getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "CaSO4") * Pollutant[w, 0] / 100;

                    CrysDens = (NCMass * 2.165 + MCMass * 2.32 + NSMass * 2.66 + CCMass * 2.15 + KCMass * 1.98 + NNMass * 1.725 + NHSMass * 1.77 + MSMass * 2.66 + KSMass * 2.66 + CaSul * 2.96) / Pollutant[w, 0];
                    CrysMass = (((4 / 3) * Math.PI * ((crysize / 10000) ^ 3) * CrysDens));
                    TotMass = (NCMass + MCMass + NSMass + CCMass + KCMass + NNMass + NHSMass + MSMass + KSMass + CaSul) / (1000 * 10000);
                    TotDrop = TotMass / CrysMass;


                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 0])
                    {
                        if (NCMass > 0)
                        {
                            NCCon = ConstData.PollFactors[1, 0] + ConstData.PollFactors[2, 0] * sensorData[w].col4 + ConstData.PollFactors[3, 0] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 0] * (Math.Pow(sensorData[w].col4, 3));
                            NCVol = (NCMass / 58.4) / NCCon;
                        }
                    }
                    else
                    {
                        NCCon = 0;
                        NCVol = 0;
                    }

                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 1])
                    {
                        if (MCMass > 0)
                        {
                            MCCon = ConstData.PollFactors[1, 1] + ConstData.PollFactors[2, 1] * sensorData[w].col4 + ConstData.PollFactors[3, 1] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 1] * (Math.Pow(sensorData[w].col4, 3));
                            MCVol = (MCMass / 95.22) / MCCon;
                        }
                    }
                    else
                    {
                        MCCon = 0;
                        MCVol = 0;
                    }
                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 2])
                    {
                        if (NSMass > 0)
                        {
                            NSCon = ConstData.PollFactors[1, 2] + ConstData.PollFactors[2, 2] * sensorData[w].col4 + ConstData.PollFactors[3, 2] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 2] * (Math.Pow(sensorData[w].col4, 3));
                            NSVol = (NSMass / 142.04) / NSCon;
                        }
                    }
                    else
                    {
                        NSCon = 0;
                        NSVol = 0;
                    }
                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 3])
                    {
                        if (CCMass > 0)
                        {
                            CCCon = ConstData.PollFactors[1, 3] + ConstData.PollFactors[2, 3] * sensorData[w].col4 + ConstData.PollFactors[3, 3] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 3] * (Math.Pow(sensorData[w].col4, 3));
                            CCVol = (CCMass / 110.98) / CCCon;
                        }
                    }
                    else
                    {
                        CCCon = 0;
                        CCVol = 0;
                    }
                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 4])
                    {
                        if (KCMass > 0)
                        {
                            KCCon = ConstData.PollFactors[1, 4] + ConstData.PollFactors[2, 4] * sensorData[w].col4 + ConstData.PollFactors[3, 4] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 4] * (Math.Pow(sensorData[w].col4, 3));
                            KCVol = (KCMass / 74.55) / KCCon;
                        }
                    }
                    else
                    {
                        KCCon = 0;
                        KCVol = 0;
                    }
                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 5])
                    {
                        if (NNMass > 0)
                        {
                            NNCon = ConstData.PollFactors[1, 5] + ConstData.PollFactors[2, 5] * sensorData[w].col4 + ConstData.PollFactors[3, 5] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 5] * (Math.Pow(sensorData[w].col4, 3));
                            NNVol = (NNMass / 80.08) / NNCon;
                        }
                    }
                    else
                    {
                        NNCon = 0;
                        NNVol = 0;
                    }
                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 6])
                    {
                        if (NHSMass > 0)
                        {
                            NHSCon = ConstData.PollFactors[1, 6] + ConstData.PollFactors[2, 6] * sensorData[w].col4 + ConstData.PollFactors[3, 6] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 6] * (Math.Pow(sensorData[w].col4, 3));
                            NHSVol = (NHSMass / 132.2) / NHSCon;
                        }
                    }
                    else
                    {
                        NHSCon = 0;
                        NHSVol = 0;
                    }
                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 7])
                    {
                        if (MSMass > 0)
                        {
                            MSCon = ConstData.PollFactors[1, 7] + ConstData.PollFactors[2, 7] * sensorData[w].col4 + ConstData.PollFactors[3, 7] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 7] * (Math.Pow(sensorData[w].col4, 3));
                            MSVol = (MSMass / 120.37) / MSCon;
                        }
                    }
                    else
                    {
                        MSCon = 0;
                        MSVol = 0;
                    }
                    if (sensorData[w].col4 >= ConstData.PollFactors[0, 8])
                    {
                        if (KSMass > 0)
                        {
                            KSCon = ConstData.PollFactors[1, 8] + ConstData.PollFactors[2, 8] * sensorData[w].col4 + ConstData.PollFactors[3, 8] * (Math.Pow(sensorData[w].col4, 2)) + ConstData.PollFactors[4, 8] * (Math.Pow(sensorData[w].col4, 3));
                            KSVol = (KSMass / 120.37) / KSCon;
                        }
                    }
                    else
                    {
                        KSCon = 0;
                        KSVol = 0;
                    }



                    TotVol = NCVol + MCVol + NSVol + CCVol + KCVol + NNVol + NHSVol + MSVol + KSVol;
                    DropVol = (TotVol * CrysMass) * 1000d;
                    DropRad = (Math.Pow((1.5d * (DropVol) / Math.PI), (1d / 3d)));
                    DropArea = Math.Pow(DropRad, 2) * Math.PI;

                    if (NCCon > 0)
                        NCCon = (NCMass / 58.4) / (TotVol * Pollutant[w, 0]);

                    if (MCCon > 0)
                        MCCon = (MCMass / 95.22) / (TotVol * Pollutant[w, 0]);

                    if (NSCon > 0)
                        NSCon = (NSMass / 142.04) / (TotVol * Pollutant[w, 0]);

                    if (NNCon > 0)
                        NNCon = (NNMass / 80.08) / (TotVol * Pollutant[w, 0]);

                    if (CCCon > 0)
                        CCCon = (CCMass / 80.08) / (TotVol * Pollutant[w, 0]);

                    if (KCCon > 0)
                        KCCon = (KCMass / 80.08) / (TotVol * Pollutant[w, 0]);

                    if (NHSCon > 0)
                        NHSCon = (NHSMass / 132.2) / (TotVol * Pollutant[w, 0]);

                    if (MSCon > 0)
                        MSCon = (MSMass / 80.08) / (TotVol * Pollutant[w, 0]);

                    if (KSCon > 0)
                        KSCon = (KSMass / 80.08) / (TotVol * Pollutant[w, 0]);

                    Chloride[w] = NCCon + 2 * MCCon + 2 * CCCon + KCCon;

                    TotCon = NCCon + MCCon + NSCon + CCCon + KCCon + NNCon + NHSCon + MSCon + KSCon;

                    if (Pollutant[w, 1] > 0.055)
                        TotCon = getLocationStats(sensorData[w].Location, sensorData[w].col6.Month, "RainConc") / (1000d * 100d);

                    TotArea = DropArea * TotDrop;
                    sensorData[w].col1 = TotArea * 0.2;

                    if (sensorData[w].col1 > 0.2)
                        sensorData[w].col1 = 0.2;

                    TOW.Add(sensorData[w].col1);

                    if (TotCon > 0)
                    {
                        if (sensorData[w].col1 >= 0.0003)
                        {
                            if (sensorData[w - 1].col1 >= 0.0003)
                            {
                                period_list[p] = period_list[p] + 1;
                                t = t + 1;


                                enviroP tempE = new enviroP();
                                double temp1;

                                tempE.col0 = sensorData[w].col2;
                                temp1 = tempE.col0;
                                tempE.col1 = sensorData[w].col4;
                                tempE.col2 = sensorData[w].col3 / 3600d;
                                tempE.col3 = TotCon;
                                tempE.col4 = DropRad / 6e-4;
                                tempE.col5 = DropArea;
                                tempE.col6 = TotArea * 100d;
                                tempE.col7 = Days;
                                tempE.col8 = TotDrop;
                                tempE.col9 = sensorData[w].col2;
                                tempE.col10 = tempE.col6 / 100d * 0.2;

                                //oxygen concentration
                                PredA = ConstData.polyConst[0, 0] + (ConstData.polyConst[1, 0] * temp1) + (ConstData.polyConst[2, 0] * (Math.Pow(temp1, 2))) + (ConstData.polyConst[3, 0] * (Math.Pow(temp1, 3)));
                                PredB1 = ConstData.polyConst[0, 1] + (ConstData.polyConst[1, 1] * temp1) + (ConstData.polyConst[2, 1] * (Math.Pow(temp1, 2))) + (ConstData.polyConst[3, 1] * (Math.Pow(temp1, 3)));
                                PredB2 = ConstData.polyConst[0, 2] + (ConstData.polyConst[1, 2] * temp1) + (ConstData.polyConst[2, 2] * (Math.Pow(temp1, 2))) + (ConstData.polyConst[3, 2] * (Math.Pow(temp1, 3)));
                                PredB3 = ConstData.polyConst[0, 3] + (ConstData.polyConst[1, 3] * temp1) + (ConstData.polyConst[2, 3] * (Math.Pow(temp1, 2))) + (ConstData.polyConst[3, 3] * (Math.Pow(temp1, 3)));

                                Oxyconc = PredA + (PredB1 * TotCon) + (PredB2 * (Math.Pow(TotCon, 2))) + (PredB3 * (Math.Pow(TotCon, 3)));
                                tempE.col11 = Oxyconc;

                                OxyconcA = (Oxyconc / 31.9998) * 1000d;
                                tempE.col12 = OxyconcA;

                                DO2 = 9.86429E-10 + 5.58571E-11 * temp1;
                                tempE.col13 = DO2;

                                //limit cathodic current density
                                Lim2024 = ((4d * 96485d * DO2 * (OxyconcA / 1000d)) / 0.001353355) * 100d;
                                tempE.col14 = Lim2024;

                                Lim7050 = ((4d * 96485d * DO2 * (OxyconcA / 1000d)) / 0.000311809) * 100d;
                                tempE.col15 = Lim7050;

                                //pit prob
                                double z1 = 0;
                                z1 = Math.Log(NCCon + 2.0d * MCCon, 10.0);
                                tempE.col16 = 8.36254E-04 - 7.48642E-04 * z1 - 5.88611E-04 * (Math.Pow(z1, 2)) - 8.75229E-05 * (Math.Pow(z1, 3));

                                if (tempE.col16 < 0)
                                    tempE.col16 = 0;

                                enviroP_List[p].Add(tempE);

                            }
                            else
                            {
                                List<enviroP> tmpList = new List<enviroP>();
                                period_list.Add(1);
                                enviroP_List.Add(tmpList);
                                p = p + 1;
                                t = 1;

                                enviroP tempE = new enviroP();
                                double temp1;

                                tempE.col0 = sensorData[w].col2;
                                temp1 = tempE.col0;
                                tempE.col1 = sensorData[w].col4;
                                tempE.col2 = sensorData[w].col3 / 3600d;
                                tempE.col3 = TotCon;
                                tempE.col4 = DropRad / 6e-4;
                                tempE.col5 = DropArea;
                                tempE.col6 = TotArea * 100d;
                                tempE.col7 = Days;
                                tempE.col8 = TotDrop;
                                tempE.col9 = sensorData[w].col2;
                                tempE.col10 = tempE.col6 / 100d * 0.2d;

                                //oxygen concentration
                                PredA = ConstData.polyConst[0, 0] + (ConstData.polyConst[1, 0] * temp1) + (ConstData.polyConst[2, 0] * (Math.Pow(temp1, 2))) + (ConstData.polyConst[3, 0] * (Math.Pow(temp1, 3)));
                                PredB1 = ConstData.polyConst[0, 1] + (ConstData.polyConst[1, 1] * temp1) + (ConstData.polyConst[2, 1] * (Math.Pow(temp1, 2))) + (ConstData.polyConst[3, 1] * (Math.Pow(temp1, 3)));
                                PredB2 = ConstData.polyConst[0, 2] + (ConstData.polyConst[1, 2] * temp1) + (ConstData.polyConst[2, 2] * (Math.Pow(temp1, 2))) + (ConstData.polyConst[3, 2] * (Math.Pow(temp1, 3)));
                                PredB3 = ConstData.polyConst[0, 3] + (ConstData.polyConst[1, 3] * temp1) + (ConstData.polyConst[2, 3] * (Math.Pow(temp1, 2))) + (ConstData.polyConst[3, 3] * (Math.Pow(temp1, 3)));

                                Oxyconc = PredA + (PredB1 * TotCon) + (PredB2 * (Math.Pow(TotCon, 2))) + (PredB3 * (Math.Pow(TotCon, 3)));
                                tempE.col11 = Oxyconc;

                                OxyconcA = (Oxyconc / 31.9998) * 1000d;
                                tempE.col12 = OxyconcA;

                                DO2 = 9.86429E-10 + 5.58571E-11 * temp1;
                                tempE.col13 = DO2;

                                //limit cathodic current density
                                Lim2024 = ((4d * 96485d * DO2 * (OxyconcA / 1000d)) / 0.001353355) * 100d;
                                tempE.col14 = Lim2024;

                                Lim7050 = ((4d * 96485d * DO2 * (OxyconcA / 1000d)) / 0.000311809) * 100d;
                                tempE.col15 = Lim7050;

                                //pit prob
                                double z1 = 0;
                                z1 = Math.Log(NCCon + 2.0d * MCCon, 10.0);
                                tempE.col16 = 8.36254E-04 - 7.48642E-04 * z1 - 5.88611E-04 * (Math.Pow(z1, 2.0)) - 8.75229E-05 * (Math.Pow(z1, 3.0));

                                if (tempE.col16 < 0)
                                    tempE.col16 = 0;

                                enviroP_List[p].Add(tempE);
                            }
                        }
                    }
                    if (w - 1 >= 1)
                    {
                        if (TOW[w - 1] >= 0.0003)
                        {
                            if (TOW[w] <= 0.0003)
                            {
                                if (period_list[p] < 3)
                                {
                                    p = p - 1;
                                }
                            }
                        }
                    }
                }//endfor
                enviroP_List[0][0].col2 = 1;
            }//endif (met type)
            else
            {//sensor data
                enviroP_List[0][0].col2 = 0;
                enviroP_List[0][0].col3 = sensorData[4].col3;

                int size = sensorData.Count;
                double days = 0;
                Pollutant = new double[size + 1, 3];
                Pollutant[0, 0] = 0;
                TOW.Add(0);
                for (z = 0; z <= size - 1; z++)
                {

                    List<enviroP> tmpEnviro = new List<enviroP>();

                    days = days + (sensorData[z].col3) / (3600d * 24d);
                    Pollutant[z, 1] = days;
                    Pollutant[z, 2] = z;
                    TOW.Add(sensorData[z].col1 - 0.0003d);

                    if (TOW[z] > 0)
                    {
                        if (TOW[z - 1] > 0)
                        {

                            double dTemp = 0;
                            double AtSatPH2O = 0;
                            double PlSatPH2O = 0;
                            double PH2O = 0;
                            double PlateRH = 0;
                            double SaltConc = 0;
                            double OxyConc = 0;
                            double OxyConcA = 0;
                            double w = 0;

                            period_list[p] = period_list[p] + 1;
                            t = t + 1;
                            //temp
                            enviroP tempEnviroP_Row = new enviroP();
                            dTemp = tempEnviroP_Row.col0 = sensorData[z].col2;
                            tempEnviroP_Row.col9 = sensorData[z].col5 + 0.2;
                            //atm rh
                            tempEnviroP_Row.col1 = sensorData[z].col4;

                            //surface RH
                            AtSatPH2O = 0.61158 + (0.04401 * tempEnviroP_Row.col9) + (0.00147 * (Math.Pow(tempEnviroP_Row.col9, 2))) + (2.58E-05 * (Math.Pow(tempEnviroP_Row.col9, 3))) + (2.86E-07 * (Math.Pow(tempEnviroP_Row.col9, 4))) + (2.72E-09 * (Math.Pow(tempEnviroP_Row.col9, 5)));
                            PlSatPH2O = 0.61158 + (0.04401 * dTemp) + (0.00147 * (Math.Pow(dTemp, 2))) + (2.58E-05 * (Math.Pow(dTemp, 3))) + (2.86E-07 * (Math.Pow(dTemp, 4))) + (2.72E-09 * (Math.Pow(dTemp, 5)));
                            PH2O = sensorData[z].col4 * AtSatPH2O / 100d;
                            PlateRH = 100d * (PH2O / PlSatPH2O);

                            if (PlateRH > 99)
                                PlateRH = 99;
                            tempEnviroP_Row.col2 = PlateRH;

                            if (PlateRH >= 60)
                            {
                                NPredA = ConstData.naClConst[0, 0] + (ConstData.naClConst[1, 0] * dTemp) + (ConstData.naClConst[2, 0] * (Math.Pow(dTemp, 2)));
                                NPredB1 = ConstData.naClConst[0, 1] + (ConstData.naClConst[1, 1] * dTemp) + (ConstData.naClConst[2, 1] * (Math.Pow(dTemp, 2)));
                                NPredB2 = ConstData.naClConst[0, 2] + (ConstData.naClConst[1, 2] * dTemp) + (ConstData.naClConst[2, 2] * (Math.Pow(dTemp, 2)));

                                SaltConc = NPredA + (NPredB1 * PlateRH) + (NPredB2 * (Math.Pow(PlateRH, 2)));
                                MaxConc = 5.4772 + (-2.4700E-03 * dTemp) + (7.7549E-05 * (Math.Pow(dTemp, 2))) + (-4.6971E-07 * (Math.Pow(dTemp, 3)));
                                MinConc = NPredA + (NPredB1 * 100) + (NPredB2 * (Math.Pow(100, 2)));

                                if (SaltConc >= MaxConc)
                                    SaltConc = MaxConc;
                                if (SaltConc <= MinConc)
                                    SaltConc = MinConc;

                                tempEnviroP_Row.col3 = SaltConc;
                            }
                            else
                            {
                                tempEnviroP_Row.col3 = 0;
                            }

                            //Ratio of crytal size to droplet size
                            if (PlateRH >= 99.5)
                                tempEnviroP_Row.col4 = 5.3;
                            else
                                tempEnviroP_Row.col4 = Math.Pow((0.566865 + (tempEnviroP_Row.col2 * -0.00564)), (-1 / 3.103932));

                            //Droplet area
                            tempEnviroP_Row.col5 = Math.Pow(((tempEnviroP_Row.col4 * Convert.ToDouble(_task.CrystalSize)) / 10000d), 2) * Math.PI;

                            //Percentage wet
                            tempEnviroP_Row.col6 = TOW[z] / 0.2 * 100;

                            //Time from start
                            tempEnviroP_Row.col7 = days;

                            //number of droplets
                            tempEnviroP_Row.col8 = (tempEnviroP_Row.col6 / 100d) / tempEnviroP_Row.col5;

                            //TOW
                            tempEnviroP_Row.col10 = tempEnviroP_Row.col8 * ((4d / 3d) * 3.14159265358979 * (Math.Pow((_task.CrystalSize / 10000d), 3)) * 2.165d) * 10000000d;
                            Pollutant[z, 0] = tempEnviroP_Row.col10;

                            //Oxygen concentration
                            PredA = ConstData.polyConst[0, 0] + (ConstData.polyConst[1, 0] * dTemp) + (ConstData.polyConst[2, 0] * Math.Pow(dTemp, 2)) + (ConstData.polyConst[3, 0] * Math.Pow(dTemp, 3));
                            PredB1 = ConstData.polyConst[0, 1] + (ConstData.polyConst[1, 1] * dTemp) + (ConstData.polyConst[2, 1] * Math.Pow(dTemp, 2)) + (ConstData.polyConst[3, 1] * Math.Pow(dTemp, 3));
                            PredB2 = ConstData.polyConst[0, 2] + (ConstData.polyConst[1, 2] * dTemp) + (ConstData.polyConst[2, 2] * Math.Pow(dTemp, 2)) + (ConstData.polyConst[3, 2] * Math.Pow(dTemp, 3));
                            PredB3 = ConstData.polyConst[0, 3] + (ConstData.polyConst[1, 3] * dTemp) + (ConstData.polyConst[2, 3] * Math.Pow(dTemp, 2)) + (ConstData.polyConst[3, 3] * Math.Pow(dTemp, 3));

                            OxyConc = PredA + (PredB1 * SaltConc) + (PredB2 * (Math.Pow(SaltConc, 2))) + (PredB3 * (Math.Pow(SaltConc, 3)));
                            tempEnviroP_Row.col11 = OxyConc;

                            OxyConcA = (OxyConc / 31.9998) * 1000;
                            tempEnviroP_Row.col12 = OxyConcA;

                            DO2 = 9.86429E-10 + 5.58571E-11 * dTemp;
                            tempEnviroP_Row.col13 = DO2;

                            //Limiting Cathodic Current Density
                            Lim2024 = ((4 * 96485d * DO2 * (OxyConcA / 1000d)) / 0.001353355) * 100d;
                            tempEnviroP_Row.col14 = Lim2024;
                            Lim7050 = ((4 * 96485d * DO2 * (OxyConcA / 1000d)) / 0.000311809) * 100d;
                            tempEnviroP_Row.col15 = Lim7050;

                            //Pit Prob


                            if (tempEnviroP_Row.col3 > 0)
                            {
                                w = Math.Log(tempEnviroP_Row.col3);
                                tempEnviroP_Row.col16 = 8.36254E-04 - 7.48642E-04 * w - 5.88611E-04 * Math.Pow(w, 2) - 8.75229E-05 * Math.Pow(w, 3);

                                if (tempEnviroP_Row.col16 < 0)
                                    tempEnviroP_Row.col16 = 0;

                            }
                            else
                                tempEnviroP_Row.col16 = 0;

                            //add object to list.
                            enviroP_List[p].Add(tempEnviroP_Row);


                        }//endif
                        else
                        {
                            List<enviroP> tempEPList = new List<enviroP>();
                            double dTemp = 0;
                            double AtSatPH2O = 0;
                            double PlSatPH2O = 0;
                            double PH2O = 0;
                            double PlateRH = 0;
                            double SaltConc = 0;
                            double OxyConc = 0;
                            double OxyConcA = 0;
                            double w = 0;
                            period_list.Add(1);

                            enviroP_List.Add(tempEPList);

                            //increase period
                            p = p + 1;
                            t = 1;

                            //Temperature 
                            enviroP tempEnviroP_Row = new enviroP();
                            //set dTemp = tmp.col0 
                            dTemp = tempEnviroP_Row.col0 = sensorData[z].col2;
                            tempEnviroP_Row.col9 = sensorData[z].col5 + 0.2;

                            //atmo RH
                            tempEnviroP_Row.col1 = sensorData[z].col4;

                            //Surface RH
                            AtSatPH2O = 0.61158 + (0.04401 * tempEnviroP_Row.col9) + (0.00147 * (Math.Pow(tempEnviroP_Row.col9, 2))) + (2.58E-05 * (Math.Pow(tempEnviroP_Row.col9, 3))) + (2.86E-07 * (Math.Pow(tempEnviroP_Row.col9, 4))) + (2.72E-09 * (Math.Pow(tempEnviroP_Row.col9, 5)));
                            PlSatPH2O = 0.61158 + (0.04401 * dTemp) + (0.00147 * (Math.Pow(dTemp, 2))) + (2.58E-05 * (Math.Pow(dTemp, 3))) + (2.86E-07 * (Math.Pow(dTemp, 4))) + (2.72E-09 * (Math.Pow(dTemp, 5)));
                            PH2O = sensorData[z].col4 * AtSatPH2O / 100;
                            PlateRH = 100 * (PH2O / PlSatPH2O);

                            if (PlateRH > 99)
                                PlateRH = 99;

                            tempEnviroP_Row.col2 = PlateRH;

                            if (PlateRH >= 78)
                            {
                                tempEnviroP_Row.col6 = 0;
                            }
                            else
                            {
                                if (PlateRH <= 70)
                                    tempEnviroP_Row.col6 = 0;
                                else
                                    tempEnviroP_Row.col6 = 1;
                            }

                            if (PlateRH >= 60)
                            {
                                //NaCL Concentration
                                NPredA = ConstData.naClConst[0, 0] + (ConstData.naClConst[1, 0] * dTemp) + (ConstData.naClConst[2, 0] * (Math.Pow(dTemp, 2)));
                                NPredB1 = ConstData.naClConst[0, 1] + (ConstData.naClConst[1, 1] * dTemp) + (ConstData.naClConst[2, 1] * (Math.Pow(dTemp, 2)));
                                NPredB2 = ConstData.naClConst[0, 2] + (ConstData.naClConst[1, 2] * dTemp) + (ConstData.naClConst[2, 2] * (Math.Pow(dTemp, 2)));

                                SaltConc = NPredA + (NPredB1 * PlateRH) + (NPredB2 * (Math.Pow(PlateRH, 2)));
                                MaxConc = 5.4772 + (-2.4700E-03 * dTemp) + (7.7549E-05 * (Math.Pow(dTemp, 2))) + (-4.6971E-07 * (Math.Pow(dTemp, 3)));
                                MinConc = NPredA + (NPredB1 * 100) + (NPredB2 * (Math.Pow(100, 2)));

                                if (SaltConc >= MaxConc)
                                    SaltConc = MaxConc;
                                if (SaltConc <= MinConc)
                                    SaltConc = MinConc;

                                tempEnviroP_Row.col3 = SaltConc;
                            }
                            else
                            {
                                tempEnviroP_Row.col3 = 0;
                            }

                            //Ratio of crystal size to droplet size
                            if (PlateRH >= 99.5)
                                tempEnviroP_Row.col4 = 5.3;
                            else
                                tempEnviroP_Row.col4 = Math.Pow((0.566865 + (tempEnviroP_Row.col2 * -0.00564)), (-1 / 3.103932));

                            //Droplet area
                            tempEnviroP_Row.col5 = Math.Pow(((tempEnviroP_Row.col4 * Convert.ToDouble(_task.CrystalSize)) / 10000), 2) * Math.PI;

                            //Percentage wet
                            tempEnviroP_Row.col6 = TOW[z] / 0.2 * 100;

                            //Time from start
                            tempEnviroP_Row.col7 = days;

                            //number of droplets
                            tempEnviroP_Row.col8 = (tempEnviroP_Row.col6 / 100) / tempEnviroP_Row.col5;

                            //TOW
                            //tempEnviroP_Row.col10 = TOW[i];
                            tempEnviroP_Row.col10 = tempEnviroP_Row.col8 * ((4 / 3) * 3.14159265358979 * (Math.Pow((_task.CrystalSize / 10000), 3)) * 2.165) * 10000000;
                            Pollutant[z, 0] = tempEnviroP_Row.col10;

                            //Oxygen concentration
                            PredA = ConstData.polyConst[0, 0] + (ConstData.polyConst[1, 0] * dTemp) + (ConstData.polyConst[2, 0] * (Math.Pow(dTemp, 2))) + (ConstData.polyConst[3, 0] * (Math.Pow(dTemp, 3)));
                            PredB1 = ConstData.polyConst[0, 1] + (ConstData.polyConst[1, 1] * dTemp) + (ConstData.polyConst[2, 1] * (Math.Pow(dTemp, 2))) + (ConstData.polyConst[3, 1] * (Math.Pow(dTemp, 3)));
                            PredB2 = ConstData.polyConst[0, 2] + (ConstData.polyConst[1, 2] * dTemp) + (ConstData.polyConst[2, 2] * (Math.Pow(dTemp, 2))) + (ConstData.polyConst[3, 2] * (Math.Pow(dTemp, 3)));
                            PredB3 = ConstData.polyConst[0, 3] + (ConstData.polyConst[1, 3] * dTemp) + (ConstData.polyConst[2, 3] * (Math.Pow(dTemp, 2))) + (ConstData.polyConst[3, 3] * (Math.Pow(dTemp, 3)));

                            OxyConc = PredA + (PredB1 * SaltConc) + (PredB2 * (Math.Pow(SaltConc, 2))) + (PredB3 * (Math.Pow(SaltConc, 3)));
                            tempEnviroP_Row.col11 = OxyConc;

                            OxyConcA = (OxyConc / 31.9998) * 1000;
                            tempEnviroP_Row.col12 = OxyConcA;

                            DO2 = 9.86429E-10 + 5.58571E-11 * dTemp;
                            tempEnviroP_Row.col13 = DO2;

                            //Limiting Cathodic Current Density
                            Lim2024 = ((4d * 96485d * DO2 * (OxyConcA / 1000)) / 0.001353355) * 100;
                            tempEnviroP_Row.col14 = Lim2024;
                            Lim7050 = ((4d * 96485d * DO2 * (OxyConcA / 1000)) / 0.00311809) * 100;
                            tempEnviroP_Row.col15 = Lim7050;

                            if (tempEnviroP_Row.col3 > 0)
                            {
                                w = Math.Log(tempEnviroP_Row.col3);
                                tempEnviroP_Row.col16 = 8.36254E-04 - 7.48642E-04 * w - 5.88611E-04 * Math.Pow(w, 2) - 8.75229E-05 * Math.Pow(w, 3);

                                if (tempEnviroP_Row.col16 < 0)
                                    tempEnviroP_Row.col16 = 0;
                            }
                            else
                                tempEnviroP_Row.col16 = 0;



                        }//endif (else)
                    }//endif

                    //because first time tow[i-1] can't be -1.
                    if (z - 1 >= 0)
                        if (TOW[z - 1] > 0)
                            if (TOW[z] < 0)
                                //period has less then 6 items - get rid of it.
                                if (period_list[p] < 6)
                                {
                                    period_list[p] = 0;
                                    //remove current element (p)
                                    period_list.RemoveAt(p);
                                    enviroP_List[p].Clear();
                                    enviroP_List.RemoveAt(p);
                                    //subtract 1 from p.
                                    p = p - 1;

                                }
                }//endfor
                if (numTasks > 1 && z == cd.cutoffValues[dropletArrayIndex])
                {
                    cd.cutoffValuesDroplets[dropletArrayIndex] = p;
                    dropletArrayIndex++;
                }
            }//endif 

            enviroP_List[0][0].col0 = p;
            enviroP_List[0][0].col1 = _task.CrystalSize;

            for (q = 1; q < p - 1; q++)
            {
                NdropsR = 0;
                CumRH = 0;
                CumTime = 0;
                AvgRad = 0;
                for (int r = 0; r < period_list[q] - 1; r++)
                {
                    if (_task.MetDataType == MetDataTypes.Meteorological)
                    {
                        CumTime = CumTime + enviroP_List[q][r].col2;
                        NdropsR = NdropsR + enviroP_List[q][r].col8 * enviroP_List[q][r].col2;
                        NdropsT = NdropsR / (Convert.ToDouble(r) + 1.0d);
                        CumRH = CumRH + enviroP_List[q][r].col1 * enviroP_List[q][r].col2;
                        CumRHT = CumRH / (Convert.ToDouble(r) + 1.0d);
                        AvgRad = AvgRad + enviroP_List[q][r].col4;
                    }
                    else
                    {
                        NdropsR = NdropsR + enviroP_List[q][r].col8;
                        NdropsT = NdropsR / (Convert.ToDouble(r) + 1.0d);
                        CumRH = CumRH + enviroP_List[q][r].col1;
                        CumRHT = CumRH / (Convert.ToDouble(r) + 1.0d);
                        AvgRad = AvgRad + enviroP_List[q][r].col4;
                    }//endif
                }//endfor


                if (_task.MetDataType == MetDataTypes.Sensor)
                {

                    nDrops nTemp = new nDrops();

                    nTemp.col0 = NdropsT;
                    nTemp.col1 = (period_list[q] * sensorData[1].col3) / 3600d;
                    nTemp.col2 = CumRHT;
                    nTemp.col3 = NdropsT * ((4d / 3d) * 3.14159265358979 * (Math.Pow((_task.CrystalSize / 10000d), 3)) * 2.165) * 10000000d;

                    nDrops.Add(nTemp);

                }
                else
                {
                    nDrops nTemp = new nDrops();
                    nTemp.col0 = NdropsR / CumTime;
                    nTemp.col1 = CumTime;
                    nTemp.col2 = CumRH / CumTime;
                    nTemp.col3 = nTemp.col0 * ((4d / 3d) * 3.14159265358979 * (Math.Pow((_task.CrystalSize / 10000d), 3)) * CrysDens) * 10000000d;

                    nDrops.Add(nTemp);

                    enviroP tmp = new enviroP();
                    tmp.col0 = nTemp.col0;
                    tmp.col1 = nTemp.col1;
                    tmp.col2 = nTemp.col2;
                    tmp.col3 = nTemp.col3;
                    tmp.col4 = enviroP_List[q][0].col7;
                    tmp.col5 = AvgRad / period_list[q]; //-1;

                    enviroP_List[0].Add(tmp);
                }
            }
            int count = 0;
            int numberOfDays = 1;
            int entriesPerDay = 0;
            DateTime currentDate = sensorData[0].col6;
            double temperatureSum = 0;
            double relativeHumiditySum = 0;
            double perciptitationSum = 0;
            double timeOfWetnessSum = 0;
            double surfacePollutantSum = 0;
            double dropletsSum = 0;
            Random rand = new Random();

            foreach (var item in sensorData)
            {
                if (item.col6.Day == currentDate.Day && item.col6.Month == currentDate.Month && item.col6.Year == currentDate.Year)
                {
                    if (savedMinDate == DateTime.MinValue)
                    {
                        savedMinDate = item.col6;
                    }
                    temperatureSum = temperatureSum + item.col2;
                    relativeHumiditySum = relativeHumiditySum + item.col4;
                    perciptitationSum = perciptitationSum + item.col5;
                    timeOfWetnessSum = timeOfWetnessSum + item.col1;
                    surfacePollutantSum = surfacePollutantSum + Pollutant[count, 0];
                    //dropletsSum = dropletsSum + (rand.Next(1, 40));
                    entriesPerDay++;
                }
                else
                {
                    tempx.Add(temperatureSum / entriesPerDay);
                    rhx.Add(relativeHumiditySum / entriesPerDay);
                    precx.Add(perciptitationSum);
                    towx.Add(timeOfWetnessSum);
                    surfacex.Add(surfacePollutantSum);
                    int drops = rand.Next(4);
                    if (drops == 3)
                    {
                        dropletsSum = (rand.NextDouble() * 5);
                    }
                    dropsx.Add(dropletsSum);
                    tempy.Add(currentDate);
                    tempy2.Add(numberOfDays);
                    numberOfDays++;

                    currentDate = item.col6;
                    temperatureSum = item.col2;
                    relativeHumiditySum = item.col4;
                    perciptitationSum = item.col5;
                    timeOfWetnessSum = item.col1;
                    surfacePollutantSum = Pollutant[count, 0];
                    dropletsSum = 0;
                    entriesPerDay = 1;
                }
                count++;
            }

            cd.Temperature = tempx;
            cd.RelativeHumidity = rhx;
            cd.Precipitation = precx;
            cd.TimeofWetness = towx;
            cd.SurfacePollutants = surfacex;
            cd.Droplets = dropsx;
            cd.Y = tempy;
            cd.DropletsY = tempy2;

            return cd;
        }//end method



        public double[,,] coelesence(ref List<List<enviroP>> _enviroP_List, AnalysisTask _task)
        {
            double RH;
            double SDensity;
            double DropCluster;
            double NoCNoD;
            double CrySize;
            double NDrops;
            double NoClust;
            int NoPits = 988;

            double DropArea;
            double WetArea;
            double ClustScale = 0;
            double LargeClust = 0;
            double WetDiff = 0;
            double DropVol = 0;
            double DropRadius = 0;

            double Yo, A1, t1, Int, B1, B2, B3, B4, P1, P2, P3, P4, eqn, Drop1, SaltPmass, y, w, k, l, m, n;
            int q;
            int x;
            int p;
            int g;
            int z;
            Yo = A1 = t1 = Int = B1 = B2 = B3 = B4 = P1 = P2 = P3 = P4 = eqn = Drop1 = SaltPmass = y = w = k = l = m = n = q = 0;

            double[] DropProb = new double[1001];
            double[] ClusterNoSize = new double[1001];
            int[] PitData = new int[1001];

            double[,] ClustSize = new double[4, 1001];
            int[] PitSDrop = new int[1001];
            ClustScale = 0;
            double[] CellSize = new double[1001];
            double[] wave0 = new double[1001];


            double[,] DCfuncVals = new double[4, 4];
            DCfuncVals[0, 0] = 3029.56;
            DCfuncVals[0, 1] = -310.24;
            DCfuncVals[0, 2] = 2360.31;
            DCfuncVals[0, 3] = -960.335;

            DCfuncVals[1, 0] = -98.6434;
            DCfuncVals[1, 1] = 8.11588;
            DCfuncVals[1, 2] = -54.734;
            DCfuncVals[1, 3] = 70.839;

            DCfuncVals[2, 0] = 1.06094;
            DCfuncVals[2, 1] = -0.0536509;
            DCfuncVals[2, 2] = -27.1784;
            DCfuncVals[2, 3] = -0.839702;

            DCfuncVals[3, 0] = -0.00377393;
            DCfuncVals[3, 1] = 0;
            DCfuncVals[3, 2] = 0;
            DCfuncVals[3, 3] = 0.00332268;

            double[,] PredG100 = new double[7, 3];
            PredG100[0, 0] = 0.990431;
            PredG100[0, 1] = -0.70891;
            PredG100[0, 2] = 0.779176;

            PredG100[1, 0] = 5.24872e-05;
            PredG100[1, 1] = -0.0024989;
            PredG100[1, 2] = -7.87883;

            PredG100[2, 0] = -6.72446e-08;
            PredG100[2, 1] = 3.9125e-06;
            PredG100[2, 2] = 557.128;

            PredG100[3, 0] = 2.36908e-11;
            PredG100[3, 1] = -1.2136e-09;
            PredG100[3, 2] = -89.0678;

            PredG100[4, 0] = 0;
            PredG100[4, 1] = 0;
            PredG100[4, 2] = 69.6577;

            PredG100[5, 0] = 0;
            PredG100[5, 1] = 0;
            PredG100[5, 2] = -1399.5;

            PredG100[6, 0] = 0;
            PredG100[6, 1] = 0;
            PredG100[6, 2] = 22.2394;



            double[,] PredG50 = new double[5, 3];
            PredG50[0, 0] = 0.994596;
            PredG50[0, 1] = -5.26042;
            PredG50[0, 2] = -38.4971;

            PredG50[1, 0] = 397.86;
            PredG50[1, 1] = 0.223299;
            PredG50[1, 2] = -8751.6;

            PredG50[2, 0] = 5.93169;
            PredG50[2, 1] = -0.00418462;
            PredG50[2, 2] = -14.5467;

            PredG50[3, 0] = 0;
            PredG50[3, 1] = 3.44337e-05;
            PredG50[3, 2] = 0;

            PredG50[4, 0] = 0;
            PredG50[4, 1] = -1.05814e-07;
            PredG50[4, 2] = 0;

            double[,] PredG25 = new double[3, 4];
            PredG25[0, 0] = -0.377491;
            PredG25[0, 1] = 0.00670168;
            PredG25[0, 2] = -2.51182e-05;
            PredG25[0, 3] = 1.99508e-08;

            PredG25[1, 0] = 0.0234332;
            PredG25[1, 1] = -0.000347875;
            PredG25[1, 2] = 1.27175e-06;
            PredG25[1, 3] = -9.4274e-10;

            PredG25[2, 0] = -0.000253511;
            PredG25[2, 1] = 5.34527e-06;
            PredG25[2, 2] = -1.66806e-08;
            PredG25[2, 3] = 1.14792e-11;

            double[,] PredG125 = new double[3, 4];
            PredG125[0, 0] = -28.6912;
            PredG125[0, 1] = -26.0461;
            PredG125[0, 2] = -0.04783;
            PredG125[0, 3] = 0.000115008;

            PredG125[1, 0] = 1.87504;
            PredG125[1, 1] = 1.4482;
            PredG125[1, 2] = 0.0078;
            PredG125[1, 3] = -1.71919e-05;

            PredG125[2, 0] = -0.0351;
            PredG125[2, 1] = -0.20629;
            PredG125[2, 2] = 0;
            PredG125[2, 3] = 7.97443e-07;

            CrySize = Convert.ToDouble(_task.CrystalSize);

            int size = Convert.ToInt32(_enviroP_List[0][0].col0) - 1;//_enviroP_List[0].Count;

            double[,,] PitString = new double[size + 3, 4, 1002];

            if (size <= 0)
            {
                NDrops = _enviroP_List[q][0].col0;
                DropRadius = (_enviroP_List[q][0].col5 / 10000d) * CrySize;
                DropArea = Math.Pow(DropRadius, 2) * Math.PI;

                WetArea = NDrops * DropArea;
                NoCNoD = 1034.95140648585d * Math.Exp(-WetArea / 0.368319971076184d) + -6.0482651588319d;

                if (NoCNoD <= 1)
                {
                    NoCNoD = 1;
                }

                if (NoCNoD >= 1000)
                {
                    NoCNoD = 1000;
                }

                PitString[1, 0, 0] = NoCNoD;
            }

            for (q = 1; q < size; q++)
            {
                RH = _enviroP_List[q][0].col2;
                SDensity = _enviroP_List[q][0].col3;
                NDrops = _enviroP_List[q][0].col0;
                DropRadius = (_enviroP_List[q][0].col5 / 10000d) * CrySize;
                DropArea = Math.Pow(DropRadius, 2) * Math.PI;
                DropVol = Math.Pow(DropRadius, 3) * Math.PI * (2.0 / 3.0);
                WetArea = NDrops * DropArea;

                NoCNoD = 1034.95140648585d * Math.Exp(-WetArea / 0.368319971076184d) + -6.0482651588319d;

                if (NoCNoD <= 1)
                {
                    NoCNoD = 1;
                }

                if (NoCNoD >= 1000)
                {
                    NoCNoD = 1000;
                }

                //layer = q, rows = 2nd, cols = 3rd
                PitString[q, 0, 0] = NoCNoD;

                if (_enviroP_List[0][0].col2 == 0)
                    if (RH < 50)
                        NoCNoD = 0;

                if (NoCNoD >= 25)
                    if (NoCNoD <= 63)
                        ClustScale = (-103.777 + 16.2082 * Math.Exp(-NoCNoD / -16.6008)) + (6.3097E-01 + 2.7252E-02 * NoCNoD - 6.3467E-04 * (Math.Pow(NoCNoD, 2))) * NDrops;
                    else if (NoCNoD <= 150)
                        ClustScale = (95.1798 + 9075.81 * Math.Exp(-NoCNoD / 21.5502)) + (1.8891E-01 + -2.5593E-03 * NoCNoD + 9.1113E-06 * (Math.Pow(NoCNoD, 2))) * NDrops;
                    else if (NoCNoD <= 750)
                        ClustScale = (8.74328 + 262.0219 * Math.Exp(-NoCNoD / 126.093)) + (3.5052E-04 + 1.5496E-01 * Math.Exp(-NoCNoD / 3.9332E+01)) * NDrops;
                    else
                        ClustScale = 0;

                Yo = 0;
                A1 = 0;
                t1 = 0;

                // Probablity values vs cluster size
                if (NoCNoD >= 100)
                {
                    Yo = PredG100[0, 0] + PredG100[1, 0] * NoCNoD + PredG100[2, 0] * (Math.Pow(NoCNoD, 2)) + PredG100[3, 0] * (Math.Pow(NoCNoD, 3));
                    A1 = PredG100[0, 1] + PredG100[1, 1] * NoCNoD + PredG100[2, 1] * (Math.Pow(NoCNoD, 2)) + PredG100[3, 1] * (Math.Pow(NoCNoD, 3));
                    t1 = PredG100[1, 2] * (Math.Exp(-NoCNoD / PredG100[2, 2])) + PredG100[3, 2] * (Math.Exp(-NoCNoD / PredG100[4, 2])) + PredG100[5, 2] * (Math.Exp(-NoCNoD / PredG100[6, 2])) + PredG100[0, 2];
                    eqn = 0;
                    Drop1 = Yo + A1 * Math.Exp(1 / t1);
                }
                else if (NoCNoD >= 50)
                {
                    Yo = PredG50[0, 0] + PredG50[1, 0] * (Math.Exp(-NoCNoD / PredG50[2, 0]));
                    A1 = PredG50[0, 1] + PredG50[1, 1] * NoCNoD + PredG50[2, 1] * (Math.Pow(NoCNoD, 2)) + PredG50[3, 1] * (Math.Pow(NoCNoD, 3)) + PredG50[4, 1] * (Math.Pow(NoCNoD, 4));
                    t1 = PredG50[0, 2] + PredG50[1, 2] * Math.Exp(NoCNoD / PredG50[2, 2]);
                    eqn = 1;
                    Drop1 = Yo + A1 * Math.Exp(1 / t1);

                }
                else if (NoCNoD >= 25)
                {
                    Int = PredG25[0, 0] + PredG25[1, 0] * NoCNoD + PredG25[2, 0] * Math.Pow(NoCNoD, 2);
                    B1 = PredG25[0, 1] + PredG25[1, 1] * NoCNoD + PredG25[2, 1] * Math.Pow(NoCNoD, 2);
                    B2 = PredG25[0, 2] + PredG25[1, 2] * NoCNoD + PredG25[2, 2] * Math.Pow(NoCNoD, 2);
                    B3 = PredG25[0, 3] + PredG25[1, 3] * NoCNoD + PredG25[2, 3] * Math.Pow(NoCNoD, 2);
                    eqn = 2;
                    Drop1 = Int + B1 + B2 + B3;

                }
                else
                {
                    eqn = 4;
                    Drop1 = 1;
                }

                DropProb[0] = 0;

                for (x = 1; x <= 1000; x++)
                {
                    if (eqn == 0)
                    {
                        DropProb[x] = Yo + A1 * Math.Exp(x / t1);

                        if (DropProb[x] > 0.9)
                            if (DropProb[x] - DropProb[x - 1] < 0.000001)
                                DropProb[x] = 1;

                        if (DropProb[x] > 1)
                            DropProb[x] = 1;
                    }
                    else if (eqn == 1)
                    {
                        DropProb[x] = Yo + A1 * Math.Exp(x / t1);

                        if (DropProb[x] > 0.9)
                            if (DropProb[x] - DropProb[x - 1] < 0.000001)
                                DropProb[x] = 1;

                        if (DropProb[x] > 1)
                            DropProb[x] = 1;
                    }
                    else if (eqn == 2)
                    {
                        DropProb[x] = Int + B1 * x + B2 * Math.Pow(x, 2) + B3 * Math.Pow(x, 3);

                        if (DropProb[x] > 0.9)
                            if (DropProb[x] - DropProb[x - 1] < 0.000001)
                                DropProb[x] = 1;

                        if (DropProb[x] > 1)
                            DropProb[x] = 1;
                    }
                    else if (eqn == 4)
                    {
                        for (int j = 1; j <= 999; j++)
                            DropProb[j] = 0;

                        DropProb[1000] = 1;
                    }


                }//endfor

                // Developing cluster information
                ClusterNoSize[0] = 0;
                w = 0;
                NoClust = 0;
                p = 0;

                for (x = 1; x <= 1000; x++)
                {
                    if (DropProb[x - 1] == 1)
                        ClusterNoSize[x] = 0;
                    else
                    {
                        ClusterNoSize[x] = (((NDrops * ((DropProb[x] - DropProb[x - 1])))) / x);

                        if (k / x >= 1)
                            ClusterNoSize[x] = ClusterNoSize[x] + (k / x);

                        if (k / x <= 1)
                            ClusterNoSize[x] = ClusterNoSize[x] + (k / x);

                        w = w + (ClusterNoSize[x] * x);
                        l = (NDrops * (DropProb[x]));
                        k = l - w;
                        NoClust = NoClust + ClusterNoSize[x];
                    }

                    if (eqn != 4)
                    {
                        if (DropProb[x - 1] == 1)
                        {
                            ClusterNoSize[x] = -1;
                            ClusterNoSize[x - 1] = ClusterNoSize[x - 2];
                            LargeClust = x - 1;
                            ClustScale = (ClustScale / LargeClust) / (LargeClust - 1);
                            x = 1000;
                        }
                    }

                }//end for

                for (x = 1; x <= LargeClust; x++)
                {
                    ClusterNoSize[x] = ClusterNoSize[x] / ((x - 1) * ClustScale + 1);
                }//end for

                //RANDOM NUMBER
                Random rnd = new Random(1);
                //Introduce pitting
                ClusterNoSize[0] = NDrops;

                //totally wet
                if (NoCNoD != 0)
                {
                    if (NoCNoD < 25)
                    {
                        for (p = 1; p <= NoPits; p++)
                        {
                            PitString[q, 0, p] = 1;
                            ClustSize[1, p] = NDrops;
                            CellSize[p] = 1;
                            wave0[p] = (ClustSize[1, p] * DropVol) / CellSize[p];
                            PitString[q, 1, p] = 1;
                            PitString[q, 2, p] = ((ClustSize[1, p] * DropVol) / CellSize[p]);
                        }
                    }
                }

                //total dry
                if (NoCNoD == 0)
                {
                    for (p = 1; p <= NoPits; p++)
                    {
                        PitString[q, 0, p] = 0;
                        ClustSize[1, p] = 0;
                        CellSize[p] = 0;
                        wave0[p] = 0;
                        PitString[q, 1, p] = 0;
                        PitString[q, 2, p] = 0;
                    }

                }

                // Isolated droplets
                if (NoCNoD > 300)
                {
                    for (p = 1; p <= NoPits; p++)
                    {
                        y = (rnd.NextDouble() + 1) / 2;
                        for (x = 1; x <= 1000; x++)
                        {
                            if (y <= DropProb[x])
                            {
                                ClustSize[0, p] = x;
                                l = (rnd.NextDouble() + 1.0) / 2.0;
                                PitData[p] = Convert.ToInt32(Math.Ceiling(l * ClusterNoSize[x]));
                                CellSize[p] = x * DropArea;
                                PitString[q, 1, p] = x * DropArea;
                                PitString[q, 2, p] = ((Math.Pow((0.566865 + (RH * -0.00564)), -1.0 / 3.103932) * CrySize) / 10000.0) * 2.0 / 3.0;
                                x = 1000;
                            }
                        }
                    }


                    g = 1;

                    for (p = 1; p <= NoPits; p++)
                    {
                        PitString[q, 0, p] = p;
                        PitSDrop[p] = 0;
                        l = 0;
                        k = 0;
                        PitString[q, 0, p] = g;

                        for (z = 1; z <= NoPits; z++)
                        {
                            if (ClustSize[0, p] == ClustSize[0, z])
                                if (PitData[p] == PitData[z])
                                    if (p != z)
                                    {
                                        PitSDrop[p] = PitSDrop[p] + 1;
                                        if (p > z)
                                        {
                                            l = 1;
                                            k = k + 1;
                                            if (k == 1)
                                                PitString[q, 0, p] = PitString[q, 0, z];
                                        }
                                    }

                        }
                        g = g + 1 - Convert.ToInt32(l);
                    }
                }//ENDIF NoCNoD > 300

                //Percolation Droplets
                if (NoCNoD <= 300)
                {
                    if (NoCNoD >= 25)
                    {
                        ClustSize[2, 0] = 0;
                        for (p = 1; p <= NoPits; p++)
                        {
                            y = (rnd.NextDouble() + 1.0) / 2.0;
                            for (x = 1; x <= DropProb[x]; x++)
                            {
                                if (y <= DropProb[x])
                                {
                                    ClustSize[0, p] = x;
                                    ClustSize[1, p] = ((ClustScale * (x - 1.0)) + 1.0) * x;
                                    ClustSize[2, p] = ClustSize[2, p - 1] + ClustSize[1, p];
                                    l = (rnd.NextDouble() + 1.0) / 2.0;
                                    PitData[p] = Convert.ToInt32(Math.Ceiling(l * ClusterNoSize[x]));

                                    x = 1000;
                                } //endif
                            }//endfor
                        }//endfor

                        g = 1;
                        for (p = 1; p < NoPits; p++)
                        {
                            PitString[q, 0, p] = p;
                            PitSDrop[p] = 0;
                            l = 0;
                            k = 0;
                            PitString[q, 0, p] = g;
                            for (z = 1; z < NoPits; z++)
                            {
                                if (ClustSize[0, p] == ClustSize[0, z])
                                {
                                    if (PitData[p] == PitData[z])
                                    {
                                        if (p != z)
                                        {
                                            PitSDrop[p] = PitSDrop[p] + 1;
                                            if (p > z)
                                            {
                                                l = 1;
                                                k = k + 1;
                                                if (k == 1)
                                                {
                                                    PitString[q, 0, p] = PitString[q, 0, z];
                                                    ClustSize[2, p] = ClustSize[2, p] - ClustSize[1, p];
                                                }//endif
                                            }//endif
                                        } //endif
                                    }//endif
                                }//endif
                            }//end for
                            g = g + 1 - Convert.ToInt32(l);
                        }//end for
                    }//endif
                }//endif

                if (ClustSize[2, 1000] >= NDrops)
                    eqn = 7;

                //large clustered droplets
                if (eqn == 7)
                {
                    if (NoCNoD >= 25)
                    {
                        g = 1;
                        m = 0;
                        n = 0;

                        ClustSize[2, 0] = 0;
                        ClustSize[3, 0] = 0;

                        for (p = 1; p <= NoPits; p++)
                        {
                            y = (rnd.NextDouble() + 1) / 2;
                            for (x = 1; x <= 1000; x++)
                            {
                                if (y <= DropProb[x])
                                {
                                    if (m != 1)
                                    {
                                        ClustSize[0, p] = 0;
                                        ClustSize[1, p] = ((ClustScale * (x - 1.0)) + 1.0) * x;
                                        ClustSize[2, p] = ClustSize[2, p - 1] + ClustSize[1, p];
                                        ClustSize[3, p] = ClustSize[3, p - 1] + (ClustSize[1, p] / NDrops);
                                        if (ClustSize[2, p] >= NDrops)
                                        {
                                            ClustSize[2, p] = NDrops;
                                            ClustSize[1, p] = NDrops - ClustSize[2, p - 1];
                                        }//endif (ClustSize[2, p] >= NDrops)

                                        l = (rnd.NextDouble() + 1.0) / 2.0;
                                        PitData[p] = Convert.ToInt32(Math.Ceiling(l * ClusterNoSize[x]));
                                        x = 1000;
                                    }//endif (m!=1)
                                }//endif (y=DropProb)
                            }//endfor (x=1)

                            if (m != 1)
                            {
                                PitString[q, 0, p] = 0;
                                PitSDrop[p] = 0;
                                l = 0;
                                k = 0;
                                PitString[q, 0, p] = g;
                                for (z = 1; z <= NoPits; z++)
                                {
                                    if (ClustSize[0, p] == ClustSize[0, z])
                                    {
                                        if (PitData[p] == PitData[z])
                                        {
                                            if (p > z)
                                            {
                                                l = 1;
                                                k = k + 1;
                                                if (k == 1)
                                                {
                                                    PitString[q, 0, p] = PitString[q, 0, z];
                                                    ClustSize[1, p] = ClustSize[1, z];
                                                    ClustSize[2, p] = ClustSize[2, p] - ClustSize[1, p];
                                                    ClustSize[3, p] = ClustSize[3, p] - (ClustSize[1, p] / NDrops);
                                                }//endif (k ==1)
                                            }//endif (p > z)
                                        }//endif (PitData[p] == PitData[z])
                                    }//endif (ClustSize[0, p] == ClustSize[0, z])
                                }//endfor (z = 1)
                                g = g + 1 - 1;
                                n = n + 1;
                            }//endif (m!=1)
                            if (ClustSize[2, p] >= NDrops)
                            {
                                m = 1;
                            }//endif  (ClustSize[2, p] >= NDrops)
                        }//endfor (p=1)
                    }//endif (NoCNoD >= 25)

                    if (m == 1)
                    {
                        for (k = n + 1; k <= NoPits; k++)
                        {
                            y = (rnd.NextDouble() + 1) / 2;
                            for (x = 1; x <= n; x++)
                            {
                                if (y <= ClustSize[3, x])
                                {
                                    PitString[q, 0, Convert.ToInt32(k)] = x;
                                    PitData[x] = PitSDrop[x] + 1;
                                    ClustSize[1, Convert.ToInt32(k)] = ClustSize[1, x];
                                    x = Convert.ToInt32(n);
                                }//endif (y <= ClustSize[3, x])
                            }//endfor (x=1)
                        }//endfor (k=n+1)
                    }//endif (m==1)

                }//endif (eqn == 7)


                if (NoCNoD <= 300)
                {
                    if (NoCNoD >= 25)
                    {
                        CellSize[0] = 0;
                        PitString[q, 2, 0] = 0;
                        PitString[q, 1, 0] = 0;
                        for (x = 1; x < 1001; x++)
                        {
                            CellSize[x] = (1.0 - (ClustSize[1, x] / NDrops * 0.291538589728257)) * ClustSize[1, x] * DropArea;
                            CellSize[0] = CellSize[0] + CellSize[x];
                            wave0[x] = (ClustSize[1, x] * DropVol) / CellSize[x];
                            PitString[q, 1, 0] = CellSize[0] + CellSize[x];
                            PitString[q, 1, x] = (1 - (ClustSize[1, x] / NDrops * 0.291538589728257)) * ClustSize[1, x] * DropArea;
                            PitString[q, 2, x] = ((ClustSize[1, x] * DropVol) / CellSize[x]);
                            PitString[q, 2, 0] = PitString[q, 2, 0] + PitString[q, 2, x];
                        }
                    }
                }

                PitString[q, 2, 0] = 0;
                PitString[q, 1, 0] = 0;
                for (x = 1; x < 1001; x++)
                {
                    PitString[q, 1, 0] = PitString[q, 1, 0] + CellSize[x];
                    PitString[q, 2, 0] = PitString[q, 2, 0] + PitString[q, 2, x];

                }

                PitString[q, 2, 0] = (PitString[q, 2, 0] / 1000.0) * 2.0 / 3.0;



            }//for (q = 1; q < size; q++)

            return PitString;

        }

        public void pit(ref List<List<enviroP>> _enviroP_List, AnalysisTask _task, double[,,] _pitString, ref CorrosionData cd)
        {
            double CrDrop, DropArea, NoCrys, a, b, d, e, g, crysize, ClustAmpi, i, st, pit, tim;
            double pitprob, temp, PRH, chloride, wetness, TfromStart, ATMtemp, TOW, oxyconc, oxyconcA, DO2, Lim2024, Lim1010, ClustD;
            double stdev, pitvariance, pitbase, pits, z, q;
            double EA;
            double DropVol, DropVolA, Steeli;


            int z1 = _enviroP_List[0].Count() + 10;

            int runs = 1;

            double[] hours = new double[1005];
            double[] pitvol = new double[1005];
            double[] pitsurf = new double[1005];
            double[] pitclust = new double[z1];
            double[,] pithisttotal = new double[60, 1000];
            double[] pithistogram = new double[60];
            double[] pitintermediate = new double[20000];
            double[,] wave0 = new double[5000, 3];
            double[,] bintotal = new double[60, 4];
            double[] largepit = new double[500];
            double[,,] wetdam = new double[200, z1, 2];
            double[] dam = new double[500];
            double[,,] Drops = new double[1005, 6, z1]; //_enviroP_List[0][0].col0
            double[,,] SteelWL = new double[(int)(_enviroP_List[0][0].col0) + 1, 3, 2]; //double[,,] SteelWL = new double[1005, 3, z1];

            double NDD2024 = 1.104570281;
            double NDD1010 = 0.311808568;


            DropVol = 1.289260401394123;
            st = 0;

            d = 0;
            pit = 0;
            int l, r, t, p;
            l = r = 0;
            a = 0;
            DropVolA = 0;

            //RANDOM NUMBER
            Random rnd = new Random(1);

            for (r = 1; r <= runs; r++)
            {
                SteelWL[l, 2, r] = 1;
                q = 0;
                pitintermediate = new double[20000];
                l = 1;

                if (_enviroP_List[0][0].col2 == 0)
                {
                    tim = _enviroP_List[0][0].col3;
                    do
                    {
                        b = 0;
                        e = 0;
                        g = 0;
                        pits = 0;
                        p = 0;
                        z = 0;
                        crysize = _pitString[l, 0, 0];

                        if (_enviroP_List[l][0].col2 >= 70)
                        {
                            DropVolA = (Math.Pow((_enviroP_List[l][0].col5 * crysize) / 10000.0, 3) * 3.14159265358979 * (2.0 / 3.0));
                        }

                        t = 0;
                        SteelWL[l, 0, r] = 0;

                        do
                        {   // CORROSION PREDITION
                            temp = _enviroP_List[l][t].col0;
                            PRH = _enviroP_List[l][t].col1;
                            tim = _enviroP_List[l][t].col2;
                            chloride = _enviroP_List[l][t].col3;
                            CrDrop = _enviroP_List[l][t].col4;
                            DropArea = _enviroP_List[l][t].col5;
                            wetness = _enviroP_List[l][t].col6;
                            TfromStart = _enviroP_List[l][t].col7;
                            NoCrys = _enviroP_List[l][t].col8;
                            ATMtemp = _enviroP_List[l][t].col9;
                            TOW = _enviroP_List[l][t].col10;
                            oxyconc = _enviroP_List[l][t].col11;
                            oxyconcA = _enviroP_List[l][t].col12;
                            DO2 = _enviroP_List[l][t].col13;
                            Lim2024 = _enviroP_List[l][t].col14;
                            Lim1010 = _enviroP_List[l][t].col15;
                            EA = wetness;

                            // 	Steel General Corrosion
                            ClustD = DropVol * _pitString[l, 2, 0];

                            if (_pitString[l, 0, 0] == 0)
                            {
                                SteelWL[l, 0, r] = 0;
                                SteelWL[l, 1, r] = st;
                                SteelWL[l, 2, r] = (1.11315690101385E-5 + 1.78129309341292E-5 * Math.Exp(-st / 4.23880161232376)) / 2.89441e-05;
                            }
                            else
                            {
                                ClustD = DropVol * _pitString[l, 2, 0];
                                ClustAmpi = (Lim1010 * NDD1010) / (10.0 * ClustD);
                                Steeli = (wetness / 100.0) * (ClustAmpi / 1e6) * (1.11315690101385E-5 + 1.78129309341292E-5 * Math.Exp(-st / 4.23880161232376) / 2.89441e-05);
                                SteelWL[l, 0, r] = SteelWL[l, 0, r] + (Steeli * tim);
                                st = st + (Steeli * tim);
                                SteelWL[l, 1, r] = st;
                                SteelWL[l, 2, r] = (1.11315690101385E-5 + 1.78129309341292E-5 * Math.Exp(-st / 4.23880161232376)) / 2.89441e-05;
                            }

                            //PIT PROBABILITY CALCS (AA2024)
                            if (_enviroP_List[0][0].col6 == 1)
                            {
                                pitprob = _enviroP_List[t][l].col16 * Math.Pow(1 - 0.57414, wetness) * 2.34817 * tim;
                                if (pitprob <= 0)
                                {
                                    pitprob = 0;
                                }
                            }
                            else
                            {
                                pitprob = 0;
                            }
                            //PROBABILISTIC PITTING
                            stdev = pitprob * 0.5;
                            pitvariance = SimpleRNG.GetNormal(); ///, stdev);
                            pitbase = pitprob + pitvariance;
                            if (pitbase < 0)
                            {
                                pitbase = 0;
                            }

                            if (pitbase > 0.2)
                            {
                                pitbase = 0.2;
                            }

                            //PIT GENERATION		
                            pits = rnd.NextDouble() + 0.5;
                            if (pits <= pitbase)
                            {
                                p = p + 1;
                                pit = pit + 1;
                                hours[p] = t / 60.0;
                                pitvol[p] = 0.000019;
                                pitsurf[p] = 2.82961E-06;
                                b = b + 1;

                                if (p >= 1000)
                                    p = 999;

                                if (b >= 1000)
                                    b = 999;

                                if (_pitString[1, 0, p] < b)
                                {
                                    b = b + 1;
                                    a = _pitString[1, 0, p];
                                }
                                else
                                {
                                    Drops[p, 0, l] = l;
                                    Drops[p, 2, l] = 2.82961E-06;
                                    Drops[p, 1, l] = _pitString[l, 1, p];
                                    g = g + 1;
                                }

                                Drops[Convert.ToInt32(a), 0, l] = Drops[Convert.ToInt32(a), 0, l] + 1;
                                Drops[Convert.ToInt32(a), 2, l] = Drops[Convert.ToInt32(a), 2, l] + 2.82961E-06;
                            }
                            //PIT GROWTH CURRENT	
                            DropVol = (Math.Pow(((Math.Pow((0.566865 + (PRH * -0.00564)), (-1 / 3.103932)) * crysize) / 10000), 3) * 3.14159265358979 * (4 / 3)) / DropVolA;
                            for (e = 1; e <= g; e += 1)
                            {
                                ClustD = DropVol * _pitString[l, 2, Convert.ToInt32(e)];
                                ClustAmpi = (Lim2024 * NDD2024) / (10 * ClustD);
                                i = ClustAmpi * Drops[Convert.ToInt32(e), 1, l];
                                if (Drops[Convert.ToInt32(e), 2, l] * 0.005 > ((((ClustAmpi) / 1e6) - 1.75e-6) * (Drops[Convert.ToInt32(e), 1, l])) + Drops[Convert.ToInt32(e), 2, l] * 0.00019)
                                {
                                    Drops[Convert.ToInt32(e), 3, l] = ((((ClustAmpi) / 1e6) - 1.75e-6) * Drops[Convert.ToInt32(e), 1, l]) / Drops[Convert.ToInt32(e), 2, l];
                                }
                                else
                                {
                                    Drops[Convert.ToInt32(e), 3, l] = 0.005;
                                }

                                //PIT GROWTH
                                Drops[Convert.ToInt32(e), 2, l] = 0;
                                if (p > 0)
                                {
                                    for (z = 1; z <= p; z++)
                                    {
                                        if (_pitString[l, 0, Convert.ToInt32(z)] == e)
                                        {
                                            pitvol[Convert.ToInt32(z)] = pitvol[Convert.ToInt32(z)] + pitsurf[Convert.ToInt32(z)] * (tim * (Drops[Convert.ToInt32(e), 3, l] + 0.00019));
                                            pitsurf[Convert.ToInt32(z)] = (Math.Pow((Math.Pow((pitvol[Convert.ToInt32(z)] * .000015906186), (1.0 / 3.0))), 2)) * 2 * 3.1415927;
                                            Drops[Convert.ToInt32(e), 2, l] = Drops[Convert.ToInt32(e), 2, l] + pitsurf[Convert.ToInt32(z)];
                                            pitintermediate[(Convert.ToInt32(q) + Convert.ToInt32(z)) - 1] = (Math.Pow((pitvol[Convert.ToInt32(z)] * .000015906186), (1.0 / 3.0))) * 20000;
                                        }//endif
                                    }//endfor
                                }//endif
                            }//endfor

                            //loops
                            t += 1;
                        } while (t < _enviroP_List[l].Count() - 1);
                        q = q + p;
                        l += 1;
                        t = 1;
                    } while (l <= _enviroP_List[0][0].col0 - 1);//do-while
                }//if
                else
                {
                    do
                    {
                        b = 0;
                        e = 0;
                        g = 0;
                        pits = 0;
                        p = 0;
                        z = 0;
                        crysize = _pitString[l, 0, 0];

                        if (_enviroP_List[0].Count == 1)
                        {
                            DropVolA = (Math.Pow((_enviroP_List[0][0].col5 * crysize) / 10000.0, 3) * 3.14159265358979 * (2.0 / 3.0));
                        }
                        else
                        {
                            DropVolA = (Math.Pow((_enviroP_List[0][l].col5 * crysize) / 10000.0, 3) * 3.14159265358979 * (2.0 / 3.0));
                        }

                        t = 0;
                        SteelWL[l, 0, r] = 0;
                        do
                        {
                            // CORROSION PREDITION
                            temp = _enviroP_List[l][t].col0;
                            PRH = _enviroP_List[l][t].col1;
                            tim = _enviroP_List[l][t].col2;
                            chloride = _enviroP_List[l][t].col3;
                            CrDrop = _enviroP_List[l][t].col4;
                            DropArea = _enviroP_List[l][t].col5;
                            wetness = _enviroP_List[l][t].col6;
                            TfromStart = _enviroP_List[l][t].col7;
                            NoCrys = _enviroP_List[l][t].col8;
                            ATMtemp = _enviroP_List[l][t].col9;
                            TOW = _enviroP_List[l][t].col10;
                            oxyconc = _enviroP_List[l][t].col11;
                            oxyconcA = _enviroP_List[l][t].col12;
                            DO2 = _enviroP_List[l][t].col13;
                            Lim2024 = _enviroP_List[l][t].col14;
                            Lim1010 = _enviroP_List[l][t].col15;
                            EA = wetness;

                            // 	Steel General Corrosion
                            ClustD = DropVol * _pitString[l, 2, 0];
                            ClustAmpi = (Lim1010 * NDD1010) / (10.0 * ClustD);
                            Steeli = (wetness / 100.0) * (ClustAmpi / 1e6) * (1.11315690101385E-5 + 1.78129309341292E-5 * Math.Exp(-st / 4.23880161232376) / 2.89441e-05);
                            SteelWL[l, 0, r] = SteelWL[l, 0, r] + (Steeli * tim);

                            st = st + (Steeli * (tim * 3600));
                            SteelWL[l, 1, r] = st;
                            SteelWL[l, 2, r] = (1.11315690101385E-5 + 1.78129309341292E-5 * Math.Exp(-st / 4.23880161232376)) / 2.89441e-05;

                            //PIT PROBABILITY CALCS (AA2024)
                            pitprob = _enviroP_List[l][t].col16 * Math.Pow(1 - 0.57414, wetness) * _enviroP_List[l][t].col2 * 3600;
                            if (pitprob <= 0)
                            {
                                pitprob = 0;
                            }

                            //PROBABILISTIC PITTING
                            stdev = pitprob * 0.5;
                            if (stdev <= 0)
                                stdev = .1;

                            pitvariance = SimpleRNG.GetNormal(1, stdev);
                            pitbase = pitprob + pitvariance;
                            if (pitbase < 0)
                            {
                                pitbase = 0;
                            }

                            if (pitbase > 1.0)
                            {
                                pitbase = 1.0;
                            }

                            //PIT GENERATION		
                            pits = rnd.NextDouble() + 0.5;
                            if (pits <= pitbase)
                            {
                                p = p + 1;
                                if (p >= 1000)
                                    p = 999;


                                b = b + 1;

                                if (b >= 1000)
                                    b = 999;
                                a = _pitString[1, 0, p];
                                pit = pit + 1;
                                hours[p] = t / 60.0;
                                pitvol[p] = 0.000019;
                                pitsurf[p] = 2.82961E-06;
                                if (a == b)
                                {
                                    Drops[Convert.ToInt32(b), 0, l] = l;
                                    Drops[Convert.ToInt32(b), 2, l] = 2.82961E-06;
                                    Drops[Convert.ToInt32(b), 1, l] = _pitString[l, 1, Convert.ToInt32(b)];
                                }
                                else
                                {
                                    Drops[Convert.ToInt32(a), 0, l] = Drops[Convert.ToInt32(a), 0, l] + 1;
                                    Drops[Convert.ToInt32(a), 2, l] = Drops[Convert.ToInt32(a), 2, l] + 2.82961E-06;
                                }
                            }

                            //PIT GROWTH CURRENT	
                            DropVol = (Math.Pow(((Math.Pow((0.566865 + (PRH * -0.00564)), (-1 / 3.103932)) * crysize) / 10000), 3) * 3.14159265358979 * (4 / 3)) / DropVolA;
                            for (e = 1; e < b; e += 1)
                            {
                                ClustD = DropVol * _pitString[l, 2, Convert.ToInt32(e)];
                                ClustAmpi = (Lim2024 * NDD2024) / (10 * ClustD);
                                i = ClustAmpi * Drops[Convert.ToInt32(e), 1, l];
                                if (Drops[Convert.ToInt32(e), 2, l] * 0.005 > ((((ClustAmpi) / 1e6) - 1.75e-6) * (Drops[Convert.ToInt32(e), 1, l])) + Drops[Convert.ToInt32(e), 2, l] * 0.00019)
                                {
                                    Drops[Convert.ToInt32(e), 3, l] = ((((ClustAmpi) / 1e6) - 1.75e-6) * Drops[Convert.ToInt32(e), 1, l]) / Drops[Convert.ToInt32(e), 2, l];
                                }
                                else
                                {
                                    Drops[Convert.ToInt32(e), 3, l] = 0.005;
                                }

                                //PIT GROWTH
                                Drops[Convert.ToInt32(e), 2, l] = 0;
                                if (p > 0)
                                {
                                    for (z = 1; z < p - 10; z++)
                                    {
                                        if (_pitString[l, 0, Convert.ToInt32(z)] == e)
                                        {
                                            pitvol[Convert.ToInt32(z)] = pitvol[Convert.ToInt32(z)] + pitsurf[Convert.ToInt32(z)] * ((_enviroP_List[l][t].col2 * 3600) * (Drops[Convert.ToInt32(e), 3, l] + 0.00019));
                                            pitsurf[Convert.ToInt32(z)] = (Math.Pow((Math.Pow((pitvol[Convert.ToInt32(z)] * .000015906186), (1.0 / 3.0))), 2)) * 2 * 3.1415927;
                                            Drops[Convert.ToInt32(e), 2, l] = Drops[Convert.ToInt32(e), 2, l] + pitsurf[Convert.ToInt32(z)];
                                            var finalValue = (Math.Pow((pitvol[Convert.ToInt32(z)] * .000015906186), (1.0 / 3.0))) * 20000;
                                            if (finalValue <= 295)
                                            {
                                                pitintermediate[((Convert.ToInt32(q) + Convert.ToInt32(z)) - 1)] = (Math.Pow((pitvol[Convert.ToInt32(z)] * .000015906186), (1.0 / 3.0))) * 20000;
                                            }
                                        }//endif
                                    }//endfor
                                }//endif
                            }//endfor


                            t += 1;
                        }
                        while (t < _enviroP_List[l].Count() - 1);//_enviroP_List[l][t].col5 > 0) ;//do
                        q = q + p;
                        ++l; // += 1;
                        t = 1;
                    } while (l <= _enviroP_List[0][0].col0 - 2);//do-while
                }//endif







            }//end for
            List<double> pit_int = new List<double>();

            pit_int = pitintermediate.Where(val => (val > 0) && !double.IsInfinity(val) && !double.IsNaN(val)).ToList();



            MathNet.Numerics.Statistics.Histogram xxx = new MathNet.Numerics.Statistics.Histogram(pit_int, 59, 10.0, 305.0);

            List<double> c_y = new List<double>();
            List<double> c_x = new List<double>();
            List<double> c_x_bottom = new List<double>();

            c_x.Add(10);

            for (int c1 = 0; c1 < xxx.BucketCount; c1++)
            {
                c_y.Add(xxx[c1].Count);

                c_x_bottom.Add(0);
                if (c1 > 0)
                    c_x.Add(c_x[c1 - 1] + 5);
            }


            List<double> c_y1 = new List<double>();

            c_y1 = c_y.Where(val => (val > 0.0)).ToList();


            cd.PitDiameter.AddRange(c_x);
            cd.PitCount.AddRange(c_y);

            int pittValue = 10;
            foreach (int value in c_y)
            {
                if (value != 0 && pittValue <= 250)
                {
                    cd.LargestPitValue = pittValue;
                }
                pittValue = pittValue + 5;
            }


            List<double> steel = new List<double>();
            List<double> enviro_4 = new List<double>();

            for (int x1 = 0; x1 < _enviroP_List[0].Count; x1++)
            {
                enviro_4.Add(_enviroP_List[0][x1].col4);
            }

            for (int x1 = 0; x1 < (int)(_enviroP_List[0][0].col0); x1++)
            {
                if (SteelWL[x1, 1, 1] > 0)
                    steel.Add(SteelWL[x1, 1, 1]);
            }

            cd.SteelWeightLoss.AddRange(steel);
            cd.SteelLossTime.AddRange(enviro_4);

        }

        public double getLocationStats(int Location, int month, string paramRequest)
        {
            double Deposit = 0;
            double Sea = 0;
            double AmmNit = 0;
            double AmmSul = 0;
            double RainDep = 0;
            double RainConc = 0;
            double NaCl = 0;
            double MgCl2 = 0;
            double Na2S04 = 0;
            double CaC12 = 0;
            double KCI = 0;
            double MgSO4 = 0;
            double K2SO4 = 0;
            double CaSO4 = 0;
            double total = 0;
            PollutionModel p = new PollutionModel();
            p = PollutionData[Location + "-" + month];

            if (Location == 1)
            {
                Deposit = p.Deposit;
                AmmNit = p.AmmNit;
                AmmSul = p.AmmSul;
                RainConc = p.RainConc;
                NaCl = p.NaCl;
                Sea = p.Sea;
                RainDep = p.RainDep;
                MgCl2 = p.MgCl2;
                Na2S04 = p.Na2S04;
                CaC12 = p.CaC12;
                KCI = p.KCI;
                MgSO4 = p.MgSO4;
                K2SO4 = p.K2SO4;
                CaSO4 = p.CaSO4;
                total = p.total;
            }
            else if (Location == 2)
            {
                Deposit = p.Deposit;
                AmmNit = p.AmmNit;
                AmmSul = p.AmmSul;
                RainConc = p.RainConc;
                NaCl = p.NaCl;
                Sea = p.Sea;
                RainDep = p.RainDep;
                MgCl2 = p.MgCl2;
                Na2S04 = p.Na2S04;
                CaC12 = p.CaC12;
                KCI = p.KCI;
                MgSO4 = p.MgSO4;
                K2SO4 = p.K2SO4;
                CaSO4 = p.CaSO4;
                total = p.total;
            }
            else if (Location == 3)
            {
                Deposit = p.Deposit;
                AmmNit = p.AmmNit;
                AmmSul = p.AmmSul;
                RainConc = p.RainConc;
                NaCl = p.NaCl;
                Sea = p.Sea;
                RainDep = p.RainDep;
                MgCl2 = p.MgCl2;
                Na2S04 = p.Na2S04;
                CaC12 = p.CaC12;
                KCI = p.KCI;
                MgSO4 = p.MgSO4;
                K2SO4 = p.K2SO4;
                CaSO4 = p.CaSO4;
                total = p.total;
            }
            else if (Location == 4)
            {
                Deposit = p.Deposit;
                AmmNit = p.AmmNit;
                AmmSul = p.AmmSul;
                RainConc = p.RainConc;
                NaCl = p.NaCl;
                Sea = p.Sea;
                RainDep = p.RainDep;
                MgCl2 = p.MgCl2;
                Na2S04 = p.Na2S04;
                CaC12 = p.CaC12;
                KCI = p.KCI;
                MgSO4 = p.MgSO4;
                K2SO4 = p.K2SO4;
                CaSO4 = p.CaSO4;
                total = p.total;
            }
            else if (Location == 5)
            {
                Deposit = p.Deposit;
                AmmNit = p.AmmNit;
                AmmSul = p.AmmSul;
                RainConc = p.RainConc;
                NaCl = p.NaCl;
                Sea = p.Sea;
                RainDep = p.RainDep;
                MgCl2 = p.MgCl2;
                Na2S04 = p.Na2S04;
                CaC12 = p.CaC12;
                KCI = p.KCI;
                MgSO4 = p.MgSO4;
                K2SO4 = p.K2SO4;
                CaSO4 = p.CaSO4;
                total = p.total;
            }
            else if (Location == 6)
            {
                Deposit = p.Deposit;
                AmmNit = p.AmmNit;
                AmmSul = p.AmmSul;
                RainConc = p.RainConc;
                NaCl = p.NaCl;
                Sea = p.Sea;
                RainDep = p.RainDep;
                MgCl2 = p.MgCl2;
                Na2S04 = p.Na2S04;
                CaC12 = p.CaC12;
                KCI = p.KCI;
                MgSO4 = p.MgSO4;
                K2SO4 = p.K2SO4;
                CaSO4 = p.CaSO4;
                total = p.total;
            }
            else if (Location == 7)
            {
                Deposit = p.Deposit;
                AmmNit = p.AmmNit;
                AmmSul = p.AmmSul;
                RainConc = p.RainConc;
                NaCl = p.NaCl;
                Sea = p.Sea;
                RainDep = p.RainDep;
                MgCl2 = p.MgCl2;
                Na2S04 = p.Na2S04;
                CaC12 = p.CaC12;
                KCI = p.KCI;
                MgSO4 = p.MgSO4;
                K2SO4 = p.K2SO4;
                CaSO4 = p.CaSO4;
                total = p.total;
            }
            else if (Location == 8)
            {
                Deposit = p.Deposit;
                AmmNit = p.AmmNit;
                AmmSul = p.AmmSul;
                RainConc = p.RainConc;
                NaCl = p.NaCl;
                Sea = p.Sea;
                RainDep = p.RainDep;
                MgCl2 = p.MgCl2;
                Na2S04 = p.Na2S04;
                CaC12 = p.CaC12;
                KCI = p.KCI;
                MgSO4 = p.MgSO4;
                K2SO4 = p.K2SO4;
                CaSO4 = p.CaSO4;
                total = p.total;
            }
            else
            {
                Deposit = 17.3;
                NaCl = 0.4;
                AmmNit = 0.025;
                AmmSul = 0.125;
                RainDep = 2.20775;
                RainConc = 5.32;
            }
            switch (paramRequest)
            {
                case "Deposit":
                    return Deposit;
                case "Sea":
                    return Sea;
                case "NaCl":
                    return NaCl;
                case "AmmNit":
                    return AmmNit;
                case "AmmSul":
                    return AmmSul;
                case "RainDep":
                    return RainDep;
                case "RainConc":
                    return RainConc;
                case "MgCl2":
                    return MgCl2;
                case "Na2S04":
                    return Na2S04;
                case "CaC12":
                    return CaC12;
                case "KCI":
                    return KCI;
                case "MgSO4":
                    return MgSO4;
                case "K2SO4":
                    return K2SO4;
                case "CaSO4":
                    return CaSO4;
                case "total":
                    return total;
                default:
                    return 0;
            }
        }
    }
}
