using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;

public class CreateExcelData : MonoBehaviour
{
    // Usage,usually it's you're generating data from the excel file and import them to the unity,The you're tweaking the values and balance them back to the excel. Typically no
    //one would ever use script to create an excel table.
    void Start()
    {
        Excel tempxls = ExcelHelper.LoadExcel(Application.dataPath + "/TestExcelFile.xlsx");
        //go and grab the sheet and add data to the console
        tempxls.ShowLog();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateExceldata()
    {
        Excel xls = new Excel();
        ExcelTable newtable = new ExcelTable();
        newtable.TableName = "TestSheet";
        xls.Tables.Add(newtable);
        string outputpath = Application.dataPath + "/TestExcelFile.xlsx";

        xls.Tables[0].SetValue(1, 1, "name");
        xls.Tables[0].SetValue(1, 2, "hp");
        xls.Tables[0].SetValue(2, 1, "Cat");
        xls.Tables[0].SetValue(2, 2, "5");

        ExcelHelper.SaveExcel(xls, outputpath);
    }
}
