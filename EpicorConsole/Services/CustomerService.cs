using EpicorConsole.Data;
using EpicorConsole.Epicor.CustomerSvc;
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

        public CustomerService(Guid sessionId)
        {
            this.sessionId = sessionId;
            builder.Path = "ERP101500/Erp/BO/Customer.svc";
            customerClient = GetClient<CustomerSvcContractClient, CustomerSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            customerClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }

        public async Task SyncPrices()
        {
            Console.WriteLine("Syncing Customers...");
            try
            {
                using (var db = new EpicorIntegrationEntities())
                {
                    var requestCustomers = db.CUSTOMERs.Where(c => c.DMSFlag == "N" || c.DMSFlag == "U");
                    CustomerTableset customerTableset = new CustomerTableset();
                    if (requestCustomers.Any())
                    {
                        foreach (var customer in requestCustomers)
                        {
                            try
                            {
                                customerClient.GetNewCustomer(ref customerTableset);
                                var customerRow = customer.DMSFlag == "N" ? customerTableset.Customer.Where(p => p.RowMod == "A").FirstOrDefault() : customerTableset.Customer.Where(p => p.CustID == customer.CustomerCode).FirstOrDefault();
                                if (customerRow != null)
                                {
                                    MapToRow(customerRow, customer);
                                }
                                customerClient.Update(ref customerTableset);
                                customer.DMSFlag = "S";
                                Console.WriteLine($"Added/Updated customer: #{customer.CustomerCode} successfully!");
                            }
                            catch (Exception e)
                            {
                                customer.DMSFlag = "F";
                                Console.WriteLine($"Added/Updated customer: #{customer.CustomerCode} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        private void MapToEntity(CUSTOMER entity, CustomerRow row)
        {
            entity.Currency = row.CurrencyCode;
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }

        private void MapToRow(CustomerRow row, CUSTOMER entity)
        {
            row.Company = !string.IsNullOrEmpty(entity.CompanyCode) ? entity.CompanyCode : row.Company;
            row.CustID = !string.IsNullOrEmpty(entity.CustomerCode) ? entity.CustomerCode : row.CustID;
            row.Name = !string.IsNullOrEmpty(entity.CustomerName) ? entity.CustomerName : row.Name;
            row.Address1 = !string.IsNullOrEmpty(entity.Address1) ? entity.Address1 : row.Address1;
            row.Address2 = !string.IsNullOrEmpty(entity.Address2) ? entity.Address2 : row.Address2;
            row.Address3 = !string.IsNullOrEmpty(entity.Address3) ? entity.Address3 : row.Address3;
            row.PhoneNum = !string.IsNullOrEmpty(entity.Phone1) ? entity.Phone1 : row.PhoneNum;
            row.FaxNum = !string.IsNullOrEmpty(entity.Fax) ? entity.Fax : row.FaxNum;
            row.EMailAddress = !string.IsNullOrEmpty(entity.Email) ? entity.Email : row.EMailAddress;
            row.CreditLimit = entity.CreditLimit.HasValue ? entity.CreditLimit.Value : row.CreditLimit;
            row.CurrencyCode = !string.IsNullOrEmpty(entity.Currency) ? entity.Currency : row.CurrencyCode;
            row.Country = !string.IsNullOrEmpty(entity.Country) ? entity.Country : row.Country;
            row.CountryNum = entity.CountryNum.HasValue ? entity.CountryNum.Value : row.CountryNum;
            row.TermsCode = !string.IsNullOrEmpty(entity.PaymentTerm) ? entity.PaymentTerm : row.TermsCode;
            row.TaxRegionCode = !string.IsNullOrEmpty(entity.GroupTax) ? entity.GroupTax : row.TaxRegionCode;
            row.ResaleID = !string.IsNullOrEmpty(entity.TaxCode) ? entity.TaxCode : row.ResaleID;
            row.GroupCode = !string.IsNullOrEmpty(entity.GroupCode) ? entity.GroupCode : row.GroupCode;
            row.ValidPayer = entity.ValidPayer;
            row.ValidShipTo = entity.ValidShipTo;
            row.ValidSoldTo = entity.ValidSoldTo;
            row.LegalName = !string.IsNullOrEmpty(entity.LegalName) ? entity.LegalName : row.LegalName;
            row.CustomerType = !string.IsNullOrEmpty(entity.CustomerType) ? entity.CustomerType : row.CustomerType;
            row.TerritoryID = !string.IsNullOrEmpty(entity.TerritoryCode) ? entity.TerritoryCode : row.TerritoryID;
            row.SalesRepCode = !string.IsNullOrEmpty(entity.SalesRepCode) ? entity.SalesRepCode : row.SalesRepCode;
            row.AllowShipTo3 = entity.AllowShipTo3;
            row.AllowOTS = entity.AllowOTS;
            row.PrintStatements = entity.PrintStatements;
            row.CreditHold = entity.CreditHold;
            row.EstDate = entity.EstablishedDate != null ? entity.EstablishedDate : row.EstDate;
            row.ConsolidateSO = entity.ConsolidateSO;
        }
    }
}
