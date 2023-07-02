using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace nightmareHunter {
    public class CSVReader : MonoBehaviour
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

    
        
        public static List<Dictionary<string, object>> Read(string file)
        {
            // var list = new List<Dictionary<string, object>>();
            // TextAsset data = Resources.Load (file) as TextAsset;
            
            // var lines = Regex.Split (data.text, LINE_SPLIT_RE);
            
            // if(lines.Length <= 1) return list;
            
            // var header = Regex.Split(lines[0], SPLIT_RE);
            // for(var i=1; i < lines.Length; i++) {
                
            //     var values = Regex.Split(lines[i], SPLIT_RE);
            //     if(values.Length == 0 ||values[0] == "") continue;
                
            //     var entry = new Dictionary<string, object>();
            //     for(var j=0; j < header.Length && j < values.Length; j++ ) {
            //         string value = values[j];
            //         value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
            //         object finalvalue = value;
            //         int n;
            //         float f;
            //         if(int.TryParse(value, out n)) {
            //             finalvalue = n;
            //         } else if (float.TryParse(value, out f)) {
            //             finalvalue = f;
            //         }
            //         entry[header[j]] = finalvalue;
            //     }
            //     list.Add (entry);
            // }
            // return list;
        // }

            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>();

            string fileName = file + ".csv";
            string filePath = Application.dataPath + "/Plugin/SaveData/" + fileName;

            // 파일 스트림 생성
             using (StreamReader reader = new StreamReader(filePath))
            {
                string headerLine = reader.ReadLine();
                string[] headers = headerLine.Split(',');
                
                while (!reader.EndOfStream)
                {
                    string dataLine = reader.ReadLine();
                    string[] values = dataLine.Split(',');
                    
                    if (values.Length == 0 || values[0] == "")
                        continue;
                    
                    Dictionary<string, object> rowData = new Dictionary<string, object>();
                    
                    for (int i = 0; i < headers.Length && i < values.Length; i++)
                    {
                        string value = values[i];
                        value = value.Trim();
                        object finalValue = value;
                        
                        int n;
                        float f;
                        
                        if (int.TryParse(value, out n))
                        {
                            finalValue = n;
                        }
                        else if (float.TryParse(value, out f))
                        {
                            finalValue = f;
                        }
                        
                        rowData[headers[i]] = finalValue;
                    }
                    
                    dataList.Add(rowData);
                }
            }

            return dataList;
        }
    }
}