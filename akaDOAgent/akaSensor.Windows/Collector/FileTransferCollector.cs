using akaCommon;
using akaCommon.Collector;
using akaSensorAgent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace akaSensor.Windows.Collector
{
    public class FileTransferCollector : AkaBackgroundWorkerBase
    {
        static private object _locker = new object();
        public const int MakerNumber = 7;
        public const int CheckerNumber = 7;
        static private List<ProcessInfo> processList = new List<ProcessInfo>();
        static private List<DataLog> dataLogs = new List<DataLog>();
        static private List<ItemInfor> _ItemsInfor = new List<ItemInfor>();

        public FileTransferCollector(ManualResetEvent resetEvent) : base(resetEvent)
        {
            object path;
        }

        /*================================|
        *====Common Functions============|
        *================================|
        */

        #region Common Functions

        public static string Unix2Dax(long unixTimeStamp)
        {
            if (unixTimeStamp != 0)
            {
                DateTime dDate = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(unixTimeStamp).ToLocalTime();
                string dateFormat = "MM/dd/yyyy_hh:mm:ss";
                return "'" + dDate.ToString(dateFormat);
            }
            else
            {
                return "N/A";
            }
        }

        private void checkDuplicateProcessInfor(List<ProcessInfo> list)
        {
        }

        #endregion Common Functions

        private void ParseJson(string strJson)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strJson))
                {
                    return;
                }
                lock (_locker)
                {
                    ItemInfor duplicateItem;
                    List<ItemInfor> _RemoveItemsInfo = new List<ItemInfor>();

                    JObject json = JObject.Parse(strJson);
                    JToken token = json.SelectToken("id");
                    JToken tokenUrl = json.SelectToken("url");
                    JToken tokenForm = json.SelectToken("form");
                    JToken tokenProcess = json.SelectToken("process");
                    JToken tokenStep = json.SelectToken("step");
                    JToken tokenisLastStep = json.SelectToken("isLastStep");
                    JToken tokenActor = json.SelectToken("actor");
                    JToken tokenStartTime = json.SelectToken("startTime");
                    JToken tokenBrowser = json.SelectToken("browser");
                    JToken tokenUser = json.SelectToken("user");
                    JToken tokenCIF = json.SelectToken("cif");
                    JToken tokenBranch = json.SelectToken("branch");

                    bool isLastStep = bool.Parse(tokenisLastStep?.ToString());

                    int cif = 0;
                    if (tokenCIF.ToString() != "")
                    {
                        cif = int.Parse(tokenCIF?.ToString());
                    }
                    int branch = int.Parse(tokenBranch?.ToString());
                    int id = 0;
                    if (token.ToString() != "")
                    {
                        id = int.Parse(token?.ToString());
                    }
                    int step = int.Parse(tokenStep?.ToString());
                    long start_time = long.Parse(tokenStartTime?.ToString());

                    string url = tokenUrl?.ToString();
                    string form = tokenForm?.ToString();
                    string process_name = tokenProcess?.ToString();
                    string actor = tokenActor?.ToString();
                    string browser = tokenBrowser?.ToString();
                    string user = tokenUser?.ToString();

                    ItemInfor InputItem = _ItemsInfor.FirstOrDefault(x => x.id == id && x.browser == browser);

                    if (token != null
                        && tokenBrowser != null
                        && tokenUrl != null
                        && tokenProcess != null
                        && tokenStartTime != null
                        && tokenCIF != null
                        && tokenUser != null
                        && tokenBranch != null)
                    {
                        /*-------------------------------------------------------------------
						 *  == INPUT DATA ==
						 */
                        if (form != null)
                        {
                            InputItem = new ItemInfor()
                            {
                                id = id,
                                url = url,
                                form_name = form,
                                process_name = process_name,
                                step = step,
                                isLaststep = isLastStep,
                                Actor = actor,
                                start_time = start_time,
                                browser = browser,
                                cif = cif,
                                branch = branch,
                                user_name = user,
                            };
                        }
                        if (InputItem.step == 1)
                        {
                            _ItemsInfor.Clear();
                            processList.Clear();

                        }

                        if (_ItemsInfor.Count == 0)
                        {
                            _ItemsInfor.Add(InputItem);
                        }
                        else if (_ItemsInfor.Count > 0)
                        {
                            //Add if not duplicate/ not exist
                            if (_ItemsInfor.Exists(x => x.step == InputItem.step &&
                             x.user_name == InputItem.user_name &&
                             x.Actor == InputItem.Actor &&
                             x.branch == InputItem.branch &&
                             x.start_time < InputItem.start_time
                            ))
                            {
                                var index = _ItemsInfor.IndexOf(_ItemsInfor.Find(x => x.step == InputItem.step &&
                               x.user_name == InputItem.user_name &&
                               x.Actor == InputItem.Actor &&
                               x.branch == InputItem.branch &&
                               x.start_time < InputItem.start_time
                                ));
                                if (index != -1)
                                    _ItemsInfor[index].start_time = InputItem.start_time;
                            }
                            else
                            {
                                _ItemsInfor.Add(InputItem);
                            }
                        }
                                                
                        var itemLastStep = _ItemsInfor.FirstOrDefault(x => x.Actor == InputItem.Actor
                        && x.user_name == InputItem.user_name
                        && x.step == InputItem.step - 1
                        && x.start_time < InputItem.start_time);
                        if (itemLastStep != null)
                        {
                            if(itemLastStep.step == 1)
                            {
                                processList.Clear();
                            }
                            ProcessInfo processInfo = new ProcessInfo()
                            {
                                id = itemLastStep.id,
                                url = itemLastStep.url,
                                form_name = itemLastStep.form_name,
                                process_name = itemLastStep.process_name,
                                step = itemLastStep.step,
                                Actor = itemLastStep.Actor,
                                start_time = itemLastStep.start_time,
                                browser = browser,
                                end_time = InputItem.start_time,
                                user_name = itemLastStep.user_name,
                                branch = itemLastStep.branch,
                            };
                            if (InputItem.cif != 0)
                            { processInfo.cif = InputItem.cif; }
                            else { processInfo.cif = itemLastStep.cif; }
                            processInfo.duration = (double)(processInfo.end_time - itemLastStep.start_time) / 1000;
                            processInfo.duration = Math.Round(processInfo.duration, 4);


                            ProcessInfo _lastProcess = new ProcessInfo();
                            if (InputItem.isLaststep)
                            {
                                _lastProcess.id = id;//
                                _lastProcess.Actor = actor;
                                _lastProcess.branch = branch;
                                _lastProcess.browser = browser;
                                _lastProcess.cif = cif;
                                _lastProcess.url = url;//
                                _lastProcess.user_name = user;
                                _lastProcess.start_time = start_time;
                                _lastProcess.step = step;
                                _lastProcess.process_name = process_name;
                                _lastProcess.form_name = form;
                                _lastProcess.isLaststep = true;
                            }

                            QueueFileProcessData(processInfo, _lastProcess);
                        }
                        if (InputItem.isLaststep)
                        {
                            _ItemsInfor.Clear();
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                AkaFocusLogger logger = new AkaFocusLogger();
                logger.LogError(ex.Message, ex);
            }
        }

        private void QueueFileProcessData(ProcessInfo processItem, ProcessInfo lastStepProcess)
        {
            string key = "";
            if(processItem.step > 0)
            {
                if (processList.Exists(x => x.step == processItem.step &&
                             x.user_name == processItem.user_name &&
                             x.Actor == processItem.Actor &&
                             x.branch == processItem.branch &&
                             x.end_time < processItem.end_time
                            ))
                {
                   var index = processList.IndexOf(processList.Find(x => x.step == processItem.step &&
                             x.user_name == processItem.user_name &&
                             x.Actor == processItem.Actor &&
                             x.branch == processItem.branch &&
                             x.end_time < processItem.end_time
                            ));
                    if (index != -1)
                    {
                        processList[index].end_time = processItem.end_time;
                        processList[index].duration = processItem.duration;
                    }
                }
                else
                {
                    processList.Add(processItem);
                }
            }
        
            if(lastStepProcess.Actor != null)
            {
                processList.Add(lastStepProcess);
            }

            if (processList.Exists(x => x.isLaststep == true))
            {
                //Get complete process
                var query = processList.FirstOrDefault(x => x.isLaststep);
                if (query != null)
                {
                    key = query.Actor;
                }
                var completeList = processList.Where(x => x.Actor == key).ToList();

                bool _isprocesscomplete = false;
                if (key.ToLower() == "maker")
                {
                    if (completeList.Count == MakerNumber)
                    {
                        _isprocesscomplete = true;
                        
                    }
                    else
                    {
                        _isprocesscomplete = false;
                    }
                }
                if (key.ToLower() == "checker")
                {
                    if (completeList.Count == CheckerNumber)
                    {
                        _isprocesscomplete = true;
                    }
                    else
                    {
                        _isprocesscomplete = false;
                    }
                }

                var cifNumber = completeList.FirstOrDefault(x => x.cif != 0);
                
                foreach (var item in completeList)
                {
                    
                    DataLog _data = new DataLog()
                    {
                        Actor = item.Actor,
                        branch = "'0"+item.branch.ToString(),
                        browser = item.browser,
                        duration = item.duration,
                        end_time = Unix2Dax(item.end_time),
                        form_name = item.form_name,
                        id = item.id,
                        process_name = item.process_name,
                        start_time = Unix2Dax(item.start_time),
                        step = item.step,
                        user_name = item.user_name,
                        url = item.url,
                        cif = cifNumber.cif,
                    };
                   
                    dataLogs.Add(_data);


                }
                if (_isprocesscomplete)
                {
                    // TODO: calculate report sum and average here
                    Reporttime report = new Reporttime() {
                        id = dataLogs.Last().id,
                        browser = dataLogs.Last().browser,
                        url = dataLogs.Last().url,
                        branch = dataLogs.Last().branch,
                        user_name = dataLogs.Last().user_name,
                        Actor = dataLogs.Last().Actor,
                        form_name = dataLogs.Last().form_name,
                        cif = dataLogs.Last().cif
                    };
                    report.process_total_time = calcProcessTotal(dataLogs);
                    report.process_avg_time = calcProcessAvg(dataLogs);
                    report.day_total_time = calcTotal(dataLogs);
                    report.day_avg_time = calcAvg(dataLogs);
                    dataLogs.Add(report);
                    processList.Clear();
                    SaveReportProcess(dataLogs);
                }
            }
        }

        private double calcProcessTotal(List<DataLog> list)
        {
            var ProcessKey = list.Last().cif;            
            return list.Where(x => x.cif == ProcessKey).Sum(item => item.duration);   
        }
        private double calcProcessAvg(List<DataLog> list)
        {
            var ProcessKey = list.Last().cif;
            return Math.Round(list.Where(x => x.cif == ProcessKey).Average(item => item.duration), 2);
        }
        private double calcTotal(List<DataLog> list)
        {
            return list.Sum(item => item.duration);
        }
        private double calcAvg(List<DataLog> list)
        {
            return Math.Round(list.Average(item => item.duration), 2);
        }

        public void SaveReportProcess(List<DataLog> list)
        {
            var json = JsonConvert.SerializeObject(list.ToArray());

            // Save json to Excel
            jsonToExcel(json);
        }

        public static void jsonToExcel(string jsonContent)
        {
            try
            {
                //string LogDir = AppInfo.DataFolder
                string LogDir = "D:/";
                string LogPath = Path.Combine(LogDir, $"{DateTime.Now.ToString("ddd-dd-MMM-yyy")}.csv");

                //used NewtonSoft json nuget package
                var dataTable = (DataTable)JsonConvert.DeserializeObject(jsonContent, (typeof(DataTable)));

                //Datatable to CSV
                var lines = new List<string>();
                string[] columnNames = dataTable.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName).
                                                  ToArray();
                var header = string.Join(",", columnNames);
                lines.Add(header);
                var valueLines = dataTable.AsEnumerable()
                                   .Select(row => string.Join(",", row.ItemArray));
                lines.AddRange(valueLines);
                File.WriteAllLines(LogPath, lines);
                if (File.Exists(LogPath))
                {
                    Console.WriteLine("Success");
                }
                else
                {
                    Console.WriteLine("Failure");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task ProcessPostDataCallback(string strJson)
        {
            await Task.Run(() =>
            {
#if DEBUG
                Console.WriteLine(strJson);
#endif
                ParseJson(strJson);
            });
        }

        protected override void ThreadProc()
        {
            try
            {
                using (var httpServer = new AkaFocusHttpServer(string.Empty, ProcessPostDataCallback))
                {
                    Task.Run(() => httpServer.Start()).Wait();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
            }
        }
    }
}