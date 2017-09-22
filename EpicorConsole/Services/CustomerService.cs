using EpicorConsole.Data;
using EpicorConsole.Epicor.CustomerSvc;
using EpicorConsole.Epicor.SessionModSvc;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicorConsole.Services
{
    public class CustomerService : BaseService
    {
        CustomerSvcContractClient customerClient;
        SessionModSvcContractClient sessionModClient;

        public CustomerService()
        {
            var sessionModService = new SessionModService();
            var sessionId = sessionModService.Login();
            this.sessionId = sessionId;
            this.sessionModClient = sessionModService.sessionModClient;
            builder.Path = $"{environment}/Erp/BO/Customer.svc";
            customerClient = GetClient<CustomerSvcContractClient, CustomerSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            customerClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }
        
        public async Task SyncCustomers()
        {
            Console.WriteLine("Syncing Customers...");
            try
            {
                using (var db = new EpicorIntergrationEntities())
                {
                    var addedCustomers = db.CUSTOMERs.Where(c => c.DMSFlag == "N");
                    //var updatedCustomers = db.CUSTOMERs.Where(c => c.DMSFlag == "U");
                    if (addedCustomers.Any()/* || updatedCustomers.Any()*/)
                    {
                        foreach (var customer in addedCustomers)
                        {
                            try
                            {
                                string siteID, siteName, workstationID, workstationDescription, employeeID, countryGroupCode, countryCode, tenantID, companyName, systemCode;

                                var currentCompany = sessionModClient.GetCurrentValues(out companyName, out siteID, out siteName, out employeeID, out workstationID, out workstationDescription, out systemCode, out countryGroupCode, out countryCode, out tenantID);

                                if (currentCompany != customer.CompanyCode)
                                {
                                    sessionModClient.SetCompany(customer.CompanyCode, out siteID, out siteName, out workstationID, out workstationDescription, out employeeID, out countryGroupCode, out countryCode, out tenantID);
                                }
                                CustomerTableset customerTableset = new CustomerTableset();
                                customerClient.GetNewCustomer(ref customerTableset);
                                var customerRow = customerTableset.Customer.Where(p => p.RowMod == "A").FirstOrDefault();
                                if (customerRow != null)
                                {
                                    MapToCustomerRow(customerRow, customer);
                                    customerClient.Update(ref customerTableset);
                                    var custNum = customerTableset.Customer[0].CustNum;
                                    var shipToRow = customerTableset.ShipTo.FirstOrDefault();
                                    if(shipToRow != null)
                                    {
                                        MapToShipToRow(shipToRow, customer);
                                        shipToRow.RowMod = "U";
                                        customerClient.Update(ref customerTableset);
                                        Console.WriteLine($"Added customer ship to 1: #{customer.CustomerCode} successfully!");
                                        //customerRow.ShipToNum = "";
                                        //customerRow.RowMod = "U";
                                        //shipToRow.ShipToNum = "";
                                        //shipToRow.RowMod = "U";
                                        //customerClient.Update(ref customerTableset);
                                        //Console.WriteLine($"Updated default ship to: #{customer.CustomerCode} successfully!");

                                        var customerShipTo = db.CUSTOMER_SHIPTO.FirstOrDefault(cst => cst.CustomerCode == customer.CustomerCode);
                                        if(customerShipTo != null)
                                        {
                                            customerClient.GetNewShipTo(ref customerTableset, custNum);
                                            var shipToRow2 = customerTableset.ShipTo.FirstOrDefault(s => s.RowMod == "A");
                                            MapToShipToRow(shipToRow2, customerShipTo);
                                            //customerRow.ShipToNum = shipToRow2.ShipToNum;
                                            //customerRow.RowMod = "U";
                                            customerClient.Update(ref customerTableset);
                                            //customerShipTo.DMSFlag = "S";
                                            Console.WriteLine($"Added customer ship to 2: #{customer.CustomerCode} successfully!");
                                        }
                                    }
                                    //customer.DMSFlag = "S";
                                    Console.WriteLine($"Added customer: #{customer.CustomerCode} successfully!");
                                }
                            }
                            catch (Exception e)
                            {
                                //customer.DMSFlag = "F";
                                customer.SystemLog = $"Added customer: #{customer.CustomerCode} failed! - {e.Message}";
                                Console.WriteLine($"Added customer: #{customer.CustomerCode} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }

                        //foreach(var customer in updatedCustomers)
                        //{
                        //    try
                        //    {
                        //        string siteID, siteName, workstationID, workstationDescription, employeeID, countryGroupCode, countryCode, tenantID, companyName, systemCode;

                        //        var currentCompany = sessionModClient.GetCurrentValues(out companyName, out siteID, out siteName, out employeeID, out workstationID, out workstationDescription, out systemCode, out countryGroupCode, out countryCode, out tenantID);

                        //        if (currentCompany != customer.CompanyCode)
                        //        {
                        //            sessionModClient.SetCompany(customer.CompanyCode, out siteID, out siteName, out workstationID, out workstationDescription, out employeeID, out countryGroupCode, out countryCode, out tenantID);
                        //        }
                        //        CustomerTableset customerTableset = customerClient.GetByCustID(customer.CustomerCode, true);
                        //        var customerRow = customerTableset.Customer.FirstOrDefault();
                        //        if (customerRow != null)
                        //        {
                        //            customerRow.RowMod = "U";
                        //            MapToCustomerRow(customerRow, customer);
                        //            customerClient.Update(ref customerTableset);
                        //            customer.DMSFlag = "S";
                        //            Console.WriteLine($"Updated customer: #{customer.CustomerCode} successfully!");
                        //        }
                        //    }
                        //    catch (Exception e)
                        //    {
                        //        //customer.DMSFlag = "F";
                        //        Console.WriteLine($"Updated customer: #{customer.CustomerCode} failed! - {e.Message}");
                        //        Console.WriteLine(e.GetBaseException().Message);
                        //        continue;
                        //    }
                        //}

                        await db.SaveChangesAsync();
                    }

                    sessionModClient.Logout();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        private void MapToShipToRow(ShipToRow row, CUSTOMER_SHIPTO entity)
        {
            row.Company = entity.CompanyCode;
            row.ShipToNum = entity.ShiptoCode;
            row.Address1 = entity.Address;
            row.Address2 = entity.Ward;
            row.Address3 = entity.District;
            row.City = entity.City;
            row.State = entity.Region;
            row.Country = entity.Country;
            row.PhoneNum = entity.Phone1;
            row.FaxNum = entity.Fax;
            row.SalesRepCode = entity.SalesRepCode;
            row.TaxRegionCode = entity.GroupTax;
            row.ShipViaCode = entity.ShipViaCode;
            row.TerritoryID = entity.TerritoryID;
            row.UserDefinedColumns["Character01"] = entity.AttrName0;
            row.UserDefinedColumns["Character02"] = entity.AttrName1;
            row.UserDefinedColumns["Character03"] = entity.AttrName2;
            row.UserDefinedColumns["Character04"] = entity.AttrName3;
            row.UserDefinedColumns["Character05"] = entity.AttrName4;
            row.UserDefinedColumns["ShortChar01"] = entity.AttrName5;
            row.UserDefinedColumns["ShortChar02"] = entity.AttrName6;
            row.UserDefinedColumns["ShortChar03"] = entity.AttrName7;
            row.UserDefinedColumns["ShortChar04"] = entity.AttrName8;
        }

        private void MapToShipToRow(ShipToRow row, CUSTOMER entity)
        {
            row.ShipToNum = "ST" + entity.CustomerCode;
            row.TerritoryID = entity.TerritoryID;
            row.UserDefinedColumns["Character01"] = entity.AttrName0;
            row.UserDefinedColumns["Character02"] = entity.AttrName1;
            row.UserDefinedColumns["Character03"] = entity.AttrName2;
            row.UserDefinedColumns["Character04"] = entity.AttrName3;
            row.UserDefinedColumns["Character05"] = entity.AttrName4;
            row.UserDefinedColumns["ShortChar01"] = entity.AttrName5;
            row.UserDefinedColumns["ShortChar02"] = entity.AttrName6;
            row.UserDefinedColumns["ShortChar03"] = entity.AttrName7;
            row.UserDefinedColumns["ShortChar04"] = entity.AttrName8;
        }

        private void MapToCustomerRow(CustomerRow row, CUSTOMER entity)
        {
            row.Company = entity.CompanyCode;
            row.CustID = entity.CustomerCode;
            row.CustomerType = entity.CustomerType;
            row.Name = entity.CustomerName;
            row.Address1 = entity.Address;
            row.Address2 = entity.Ward;
            row.Address3 = entity.District;
            row.City = entity.City;
            row.State = entity.Region;
            row.Country = entity.Country;
            row.SalesRepCode = entity.SalesRepCode;
            row.PhoneNum = entity.Phone1;
            row.FaxNum = entity.Fax;
            row.TaxRegionCode = entity.GroupTax;
            row.ResaleID = entity.TaxCode;
            row.CurrencyCode = entity.Currency;
            row.FederalID = entity.BusinessRegistrationCertificate;
            row.ShipViaCode = entity.ShipViaCode;
            row.CreditLimit = entity.CreditLimit;
            row.TermsCode = entity.PaymentTerm;
            row.TerritoryID = entity.TerritoryID;
            row.ShipToNum = "ST" + entity.CustomerCode;
            row.UserDefinedColumns["Character01"] = entity.FullAddress;
        }
    }
}
