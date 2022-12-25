using CsQuery;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            var pathFile = @"D:\Программирование\dromParse\1.txt";
            var pathCatalog = "https://www.drom.ru/catalog/";
            var pathCatalogH = "https://www.drom.ru/catalog/dodge/";
            StringBuilder sb = new StringBuilder();
            var genStr = "";
            var chetIzg = 0;
            string[] dataTmp = new string[100]; //массив строк для временного хранения строк
            string[] dataIzg = new string[100]; //массив для записи строк даты изготовления и кузова
            string[] modDvig = new string[100]; //массив для записи строк модель, объём и двигатель
            string[] ls1 = new string[100];  //массив для запси лошадинных сил
            var markModel = "";

            CQ domMark = CQ.CreateFromUrl(pathCatalogH);
            foreach (IDomObject mark in domMark.Find(".css-2u02es a"))
            {
                var pathModel = "";

                pathModel = pathCatalog + mark.GetAttribute("href").Substring(28);
                CQ domModel = CQ.CreateFromUrl(pathModel);

                foreach (IDomObject model in domModel.Find(".b-random-group_margin_r-b-size-s-s a"))
                {

                    var modelVar = "";
                    var chet = 0;
                    modelVar = pathCatalog + model.GetAttribute("href").Substring(9);
                    CQ domVar = CQ.CreateFromUrl(modelVar);
                    
                    //Ищу поколение
                    foreach (IDomObject genFind in domVar.Find(".b-title_type_h1"))
                    {
                        var newStr = genFind.Cq().Text();
                        Regex regex = new Regex(@"\d{1}\sпоколение|\d\sПоколение");
                        MatchCollection matches = regex.Matches(newStr);
                        if (matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                genStr = ";" + match;
                            }

                        }
                        else
                        {
                            //Console.WriteLine("Совпадений data не найдено");
                        }
                    }

                    chet = 0;
                    var chetGoal = 0;
                    foreach (IDomObject goal in domVar.Find(".b-table_align_center td"))
                    {

                        if (goal.InnerText != "")
                        {

                            var strGoal = goal.InnerText.Replace(" ", "").Replace("&nbsp;", " ").Replace("\n", "").Trim();
                            Regex regex = new Regex(@"^\d{2}.\d{4}\s-");
                            MatchCollection matches = regex.Matches(strGoal);

                            if (matches.Count > 0)
                            {
                                foreach (Match match in matches)
                                {
                                    dataIzg[chetGoal] = " " + strGoal;
                                }

                            }
                            else
                            {
                                //Console.WriteLine("Совпадений data не найдено");
                            }


                            Regex regexMod = new Regex(@"^\D{1}");
                            MatchCollection matchesMod = regexMod.Matches(strGoal);

                            if (matchesMod.Count > 0)
                            {
                                foreach (Match match in matchesMod)
                                {
                                    dataIzg[chetGoal] = dataIzg[chetGoal] + ";" + strGoal;
                                    chetGoal++;
                                }

                            }
                            else
                            {
                                //Console.WriteLine("Совпадений mod не найдено");
                            }
                            chet++;
                        }
                    }
                    chetGoal = 0;

                    //Очищаю массив строк для временного хранения строк
                    chet = 0;
                    foreach (var strTmp in dataTmp)
                    {
                        dataTmp[chet] = "";
                        chet++;
                    }

                    chet = 0;
                    foreach (IDomObject goal2 in domVar.Find(".b-table_align_center a"))
                    {
                        Regex regex = new Regex(@"^\d.\d|^\d{2}.\d{2}|\d.\d{2}");
                        MatchCollection matches = regex.Matches(goal2.Cq().Text());
                        if (matches.Count > 0)
                        {
                            foreach (Match match in matches)
                            {
                                modDvig[chet] = " " + goal2.Cq().Text() + " ";
                            }
                        }
                        else
                        {
                            modDvig[chet] = modDvig[chet] + "(" +goal2.Cq().Text() + ")";
                            chet++;
                        }
                        var pathLs = pathCatalog + goal2.GetAttribute("href").Substring(9);
                        CQ domLs = CQ.CreateFromUrl(pathLs);
                        foreach (IDomObject ls in domLs.Find(".b-model-specs__text")) 
                        {
                            if (ls.Cq().Text().Contains("л.с."))
                            {
                                //вместо .innerText нужно обходить .Cq().Text() что бы корректно получать русские буквы
                                var strLs = ls.Cq().Text().Replace("\n", "").Replace(" ", "").Trim();

                                Regex regexLs = new Regex(@"\d{3}|\d{2}");
                                MatchCollection matchesLs = regexLs.Matches(strLs);
                                var chetUntill = 0;
                                if (matches.Count > 0)
                                {
                                    foreach (Match matchLs in matchesLs)
                                    {
                                        ls1[chet] = matchLs.Value.ToString().Trim() + " л.с."; ;
                                        chetUntill++;
                                    }
                                    chetUntill = 0;
                                }
                                else
                                {
                                    Console.WriteLine("Совпадений не найдено ls");
                                }
                            }
                        }
                        

                    }

                    //Очищаю массив строк для временного хранения строк
                    chet = 0;
                    foreach (var strTmp in dataTmp)
                    {
                        dataTmp[chet] = "";
                        chet++;
                    }
                                       
                    chet = 0;
                    foreach (IDomObject markM in domVar.Find(".css-185nwj5 a"))
                    {
                        chet++;
                        switch (chet)
                        {
                            case 4:
                                markModel = markM.GetAttribute("href").Substring(9).Replace("/", ";");
                                markModel = markModel.Substring(0, markModel.Length - 1);
                                break;
                        }
                        //Console.WriteLine(markModel);
                    }
                    chet = 0;
                    foreach (var i in modDvig)
                    {
                        if (modDvig[chet] != "")
                        {
                            sb.Clear();
                            sb.Append(markModel);
                            sb.Append(modDvig[chet]);
                            sb.Append(dataIzg[chet]);
                            sb.Append("; ");
                            if (ls1[chet] != null)
                            {
                                var lsWatt = int.Parse(ls1[chet].Substring(0, ls1[chet].Length - 4));
                                var result = lsWatt * 0.735;
                                result = Math.Round(result, 0);
                                sb.Append(result);
                            }

                            sb.Append(";");
                            sb.Append(ls1[chet]);
                            sb.Append(genStr);

                            if (sb.ToString().Contains(markModel + modDvig + "; ;" + genStr)| sb.ToString().Contains(markModel + "; ;" + genStr)| sb.ToString().Contains(markModel + modDvig + "(); " + "; ;" + genStr))
                            {
                                sb.Clear();
                                break;
                            }
                            using (FileStream fs = new FileStream(pathFile, FileMode.Append))
                            {
                                byte[] arrey = Encoding.UTF8.GetBytes(sb.ToString() + "\r\n");
                                fs.Write(arrey, 0, arrey.Length);
                            }

                            Console.WriteLine(sb.ToString());
                            sb.Clear();
                        }
                        chet++;
                    }
                }

            }

        }
    }
}
