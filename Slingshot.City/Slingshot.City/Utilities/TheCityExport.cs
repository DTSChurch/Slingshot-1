using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
 using System.Data.OleDb;
 using System.Globalization;


using Slingshot.Core;
using Slingshot.Core.Model;
using Slingshot.Core.Utilities;

using Slingshot.City.Utilities.Translators;

namespace Slingshot.City.Utilities
{
    public static class TheCityExport
    {

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        /// <value>
        /// The file name.
        /// </value>
        public static string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file name.
        /// </value>
        public static string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public static string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the person attributes
        /// </summary>
        public static Dictionary<string, string> PersonAttributes { get; set; }


        /// <summary>
        /// Gets or sets the grouptypes
        /// </summary>
        public static Dictionary<string, int> GroupTypes { get; set; }

        /// <summary>
        /// Gets or sets the locations
        /// </summary>
        public static DataTable dtLocations { get; set; }


        /// <summary>
        /// Gets or sets the group members
        /// </summary>
        public static DataTable dtGroupMembers { get; set; }

        /// <summary>
        /// Get or set group attributes 
        /// </summary>
        public static Dictionary<string, string> GroupAttributes { get; set; }

        /// <summary>
        /// Get or set batch totals by batch id
        /// </summary>
        public static Dictionary<Int32, decimal> BatchTotals { get; set; }


        /// <summary>
        /// exports person records.
        /// </summary>
        /// <param name="fqp"></param>
        public static void ExportIndividuals()
        {
            try
            {
                //load csv to data table               
                string PersonFilePath = FilePath + "Persons";
                string[] file = Directory.GetFiles(PersonFilePath, "*.csv");
                string fileName = file[0].ToString();
                DataTable dtPersons = GetDataTableFromCsv(fileName, true);
                //export people to translator
                if (dtPersons != null)
                {
                    //cleaning up the data a bit
                    foreach (DataRow row in dtPersons.Rows)
                    {
                        //clean up dates                      
                        row["created_at"] = ParseDateTime(row["created_at"].ToString());
                        row["birthdate"] = ParseDateTime(row["birthdate"].ToString());
                        if (string.IsNullOrEmpty(row["family_id"].ToString().Trim()) == true)
                        {
                            //add 1 to max value
                            int id = Convert.ToInt32(dtPersons.Compute("max([family_id])", string.Empty));
                            row["family_id"] = id + 1;
                        }
                        //phone types
                        string secondPhoneType = "";
                        switch (row["secondary_phone_type"].ToString().ToUpper().Trim())
                        {
                            case "HOME":
                                secondPhoneType = "Home";
                                break;
                            case "WORK":
                                secondPhoneType = "Work";
                                break;
                            case "MOBILE":
                                secondPhoneType = "Mobile";
                                break;
                            default:
                                secondPhoneType = row["secondary_phone_type"].ToString().ToUpper().Trim();
                                break;
                        }
                        row["secondary_phone_type"] = secondPhoneType;
                        string primaryPhoneType = "";
                        switch (row["primary_phone_type"].ToString().ToUpper().Trim())
                        {
                            case "HOME":
                                primaryPhoneType = "Home";
                                break;
                            case "WORK":
                                primaryPhoneType = "Work";
                                break;
                            case "MOBILE":
                                primaryPhoneType = "Mobile";
                                break;
                            default:
                                primaryPhoneType = row["primary_phone_type"].ToString().ToUpper().Trim();
                                break;
                        }
                        row["primary_phone_type"] = primaryPhoneType;
                    }
                    //load attributes
                    LoadPersonAttributes(dtPersons);
                    // write out the person attributes
                    WritePersonAttributes();
                    foreach (DataRow r in dtPersons.Rows)
                    {
                        var importPerson = CityPerson.Translate(r);
                        if (importPerson != null)
                        {
                            ImportPackage.WriteToPackage(importPerson);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        /// <summary>
        /// Exporting financial data
        /// </summary>
        public static void ExportFinancials()
        {
            try
            {
                //load batches, donations, funds csv's to datatables first.
                string FinancialFilePath = FilePath + @"Financial\";
                DataTable dtBatches = GetDataTableFromCsv(FinancialFilePath + "batches.csv", true);
                DataTable dtDonations = GetDataTableFromCsv(FinancialFilePath + "donations.csv", true);
                DataTable dtFunds = GetDataTableFromCsv(FinancialFilePath + "funds.csv", true);

                if(dtFunds != null)
                {
                    foreach(DataRow row in dtFunds.Rows)
                    {
                        var importAccount = CityFinancialAccount.Translate(row);
                        if(importAccount != null)
                        {
                            ImportPackage.WriteToPackage(importAccount);
                        }
                    }
                }
                if(dtBatches != null)
                {
                    BatchTotals = new Dictionary<Int32, decimal>();
                    foreach (DataRow row in dtBatches.Rows)
                    {
                        /*
                        //sum of donations by batch id.
                        string filter = "batch_id = " + row["id"].ToString();
                        object sumObject;
                        sumObject = dtDonations.Compute("Sum(amount)", filter);                                   
                        BatchTotals.Add(Convert.ToInt32(row["id"].ToString()),Convert.ToDecimal(sumObject.ToString()));
                        */    
                        
                        //clean up dates if needed
                        var importBatches = CityFinancialBatch.Translate(row);
                        if(importBatches != null)
                        {
                            ImportPackage.WriteToPackage(importBatches);
                        }
                    }
                }
                if(dtDonations != null)
                {
                    //financial transactions
                    foreach (DataRow row in dtDonations.Rows)
                    {
                        var importDonations = CityFinancialTransaction.Translate(row);
                        var importDonationDetails = CityFinancialTransactionDetails.Translate(row);
                        if(importDonations != null)
                        {
                            ImportPackage.WriteToPackage(importDonations);
                        }
                        if(importDonationDetails != null)
                        {
                            ImportPackage.WriteToPackage(importDonationDetails);
                        }
                    }
                }
                 
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Exports any groups found.  
        /// </summary>
        public static void ExportGroups()
        {
            try
            {            
                //load csv to data table               
                string PersonFilePath = FilePath + "Groups";
                string[] file = Directory.GetFiles(PersonFilePath, "*.csv");
                string fileName = file[0].ToString();
                //Get/set groups
                using (var dtGroups = GetDataTableFromCsv(fileName, true))
                {
                    //loop through datatable for group types first
                    WriteGroupTypes(dtGroups);
                    WriteLocations(dtGroups);
                    
                    foreach (DataRow row in dtGroups.Rows)
                    {
                        //ensure no blank parent group id's.
                        if (string.IsNullOrEmpty(row["parent_group_id"].ToString().Trim()) == true)
                        {
                            //add 1 to max value
                            int id = Convert.ToInt32(dtGroups.Compute("max([parent_group_id])", string.Empty));
                            row["parent_group_id"] = id + 1;
                        }
                        var importGroup = CityGroup.Translate(row);
                        if (importGroup != null)
                        {
                            ImportPackage.WriteToPackage(importGroup);
                        }
                    }
                }

                //Loop through each group's file for members and id's. 
                dtGroupMembers = GetGroupMemberDataTable();
                string GroupMemberFile = FilePath + "GroupMembers";
                file = Directory.GetFiles(GroupMemberFile, "*.csv");
                foreach(string foundFile in file)
                {
                    using (var dtGroupMember = GetDataTableFromCsv(foundFile, true))
                    {   
                        if(dtGroupMember != null)
                        {
                            DataView view = new DataView(dtGroupMember);
                            DataTable distinctRecords = view.ToTable(true, "id", "role_in_group", "group_id");
                            foreach (DataRow rw in distinctRecords.Rows)
                            {
                                if(rw["role_in_group"].ToString().Trim() != "")
                                {
                                    dtGroupMembers.Rows.Add(rw["id"].ToString(), rw["role_in_group"].ToString(), rw["group_id"].ToString());
                                }                             
                            }
                        }
                    }
                }
                if(dtGroupMembers != null)
                {
                    DataView view = new DataView(dtGroupMembers);
                    DataTable distinctRecords = view.ToTable(true, "id", "role_in_group", "group_id");
                    foreach (DataRow rw in distinctRecords.Rows)
                    {
                        var importGroup = CityGroupMembers.Translate(rw);
                        if (importGroup != null)
                        {
                            ImportPackage.WriteToPackage(importGroup);
                        }
                    }
                }               
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Writes the group types.
        /// </summary>
        public static void WriteGroupTypes(DataTable dt)
        {
            GroupTypes = new Dictionary<string, int>();
            var distinctNames = (from row in dt.AsEnumerable()
                                 select row.Field<string>("group_type")).Distinct();
            int GroupTypeId = 0;
            string GroupTypeName = "";
            foreach (var name in distinctNames)
            {
                //Assign new id, store into list, and write to package.
                GroupTypeId = GroupTypeId+1;
                GroupTypeName = name.ToString();
                GroupTypes.Add(GroupTypeName, GroupTypeId);
                ImportPackage.WriteToPackage(new GroupType()
                {
                    Id = GroupTypeId,
                    Name = GroupTypeName
                });
            }
        }

        public static void WriteLocations(DataTable dtLocs)
        {
            //get distinct locations and create unique id's.
            dtLocations = GetLocationsDataTable();

            DataView view = new DataView(dtLocs);
            DataTable distinctLocations = view.ToTable(true, "street", "street2","city","state","zipcode");
            int LocId = 0;

            foreach (DataRow row in distinctLocations.Rows)
            {
                string s = row["street"].ToString() + row["street2"].ToString() + row["city"].ToString() + row["state"].ToString() + row["zipcode"].ToString();
                if (!string.IsNullOrEmpty(s.Trim()))
                {
                    LocId = LocId + 1;
                    dtLocations.Rows.Add(LocId, row["street"].ToString(), row["street2"].ToString(), row["city"].ToString(), row["state"].ToString(), row["zipcode"].ToString());
                }
            }
            if (dtLocations != null)
            {
                foreach (DataRow row in dtLocations.Rows)
                {
                    var importLocations = CityLocation.Translate(row);
                    if (importLocations != null)
                    {
                        ImportPackage.WriteToPackage(importLocations);
                    }
                }

            }
        }

        //convert csv file to datatable
        static DataTable GetDataTableFromCsv(string path, bool isFirstRowHeader)
        {
            string header = isFirstRowHeader ? "Yes" : "No";
            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            string sql = @"SELECT * FROM [" + fileName + "]";

            using (OleDbConnection connection = new OleDbConnection(
                      @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathOnly +
                      ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand command = new OleDbCommand(sql, connection))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                adapter.FillSchema(dataTable, SchemaType.Source);
                //change column data types here
                DataColumnCollection columns = dataTable.Columns;
                if (columns.Contains("zipcode"))
                {
                    dataTable.Columns["zipcode"].DataType = typeof(string);
                }             
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        //clean up invalid datetime vars
        static DateTime ParseDateTime(string str)
        {
            DateTime dDate = DateTime.MinValue;
            if (!DateTime.TryParse(str, out dDate))
            {
                dDate = DateTime.MinValue;
            }
            return dDate;
        }
        //creat locations datatable
        //"street", "street2","city","state","zipcode"
        static DataTable GetLocationsDataTable()
        {
            DataTable dtLocations = new DataTable();
            dtLocations.Columns.Add("id", typeof(int));
            dtLocations.Columns.Add("street", typeof(string));
            dtLocations.Columns.Add("street2", typeof(string));
            dtLocations.Columns.Add("city", typeof(string));
            dtLocations.Columns.Add("state", typeof(string));
            dtLocations.Columns.Add("zipcode", typeof(string));
            return dtLocations;
        }
        static DataTable GetGroupMemberDataTable()
        {
            DataTable dtGroupMembers = new DataTable();
            dtGroupMembers.Columns.Add("id", typeof(int));
            dtGroupMembers.Columns.Add("role_in_group", typeof(string));
            dtGroupMembers.Columns.Add("group_id", typeof(int));
            return dtGroupMembers;

        }
        //get person attributes
        public static void LoadPersonAttributes(DataTable dt)
        {
            /* 
             * List all the feilds currently mapped in all of the person files 
             * (i.e., person.csv, person-address.csv, person-phone.csv) all the fields which aren't mapped will
             *  come in as person-attributes
             *  
             */
            string[] excludedCols =
            {    "id"
                ,"first"
                ,"middle"
                ,"last"
                ,"preferred_name"
                ,"email"
                ,"birthdate"
                ,"gender"
                ,"family_id"
                ,"family_role"
                ,"active"
                ,"member"
                ,"created_at"
                ,"marital_status"
                ,"street"
                ,"street2"
                ,"city"
                ,"state"
                ,"zipcode"
                ,"location_type"
                ,"primary_phone_type"
                ,"primary_phone"
                ,"secondary_phone_type"
                ,"secondary_phone"
            };
            //check to see which columns AREN'T already mapped to any of the translators.
            //if any found, add to person attributes.
            PersonAttributes = new Dictionary<string, string>();
            foreach (DataColumn col in dt.Columns)
            {
                int pos = Array.IndexOf(excludedCols, col.ColumnName.ToLower().Trim());
                if (pos == -1)
                {
                    PersonAttributes.Add(col.ColumnName, col.DataType.Name);
                }
            }
        }

        public static void LoadGroupAttributes(DataTable dtGroupAttributes)
        {
            /* 
             * List all the feilds currently mapped in all of the group files 
             * (i.e., person.csv, person-address.csv, person-phone.csv) all the fields which aren't mapped will
             *  come in as group-attributes
             *  
             */
            string[] excludedCols =
            {
                "group_type"
                ,"parent_group"
                ,"name"
                ,"id"
                ,"street"
                ,"street2"
                ,"city"
                ,"state"
                ,"zipcode"
            };
            //check to see which columns AREN'T already mapped to any of the translators.
            //if any found, add to person attributes.
            GroupAttributes = new Dictionary<string, string>();
            foreach (DataColumn col in dtGroupAttributes.Columns)
            {
                int pos = Array.IndexOf(excludedCols, col.ColumnName.ToLower().Trim());
                if (pos == -1)
                {
                    PersonAttributes.Add(col.ColumnName, col.DataType.Name);
                }
            }
        }
        //write person attributes
        public static void WritePersonAttributes()
        {
            foreach (var attrib in PersonAttributes)
            {
                var attribute = new PersonAttribute();
                // strip out "Ind" from the attribute name and add spaces between words
                attribute.Name = ExtensionMethods.SplitCase(attrib.Key.Replace("Ind", ""));
                attribute.Key = attrib.Key;
                attribute.Category = "Imported Attributes";
                switch (attrib.Value)
                {
                    case "String":
                        attribute.FieldType = "Rock.Field.Types.TextFieldType";
                        break;
                    case "DateTime":
                        attribute.FieldType = "Rock.Field.Types.DateTimeFieldType";
                        break;
                    default:
                        attribute.FieldType = "Rock.Field.Types.TextFieldType";
                        break;
                }
                ImportPackage.WriteToPackage(attribute);
            }
        }
    }
}
