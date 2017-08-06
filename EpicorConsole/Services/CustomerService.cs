using EpicorConsole.Data;
using EpicorConsole.Epicor.CustomerSvc;
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

        public CustomerService(Guid sessionId)
        {
            this.sessionId = sessionId;
            builder.Path = "ERP101500/Erp/BO/Customer.svc";
            customerClient = GetClient<CustomerSvcContractClient, CustomerSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            customerClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }

        [DisableConcurrentExecution(100000)]
        public async Task SyncCustomers()
        {
            Console.WriteLine("Syncing Customers...");
            try
            {
                using (var db = new EpicorIntegrationEntities())
                {
                    var addedCustomers = db.CUSTOMERs.Where(c => c.DMSFlag == "N");
                    var updatedCustomers = db.CUSTOMERs.Where(c => c.DMSFlag == "U");
                    if (addedCustomers.Any() || updatedCustomers.Any())
                    {
                        foreach (var customer in addedCustomers)
                        {
                            try
                            {
                                CustomerTableset customerTableset = new CustomerTableset();
                                customerClient.GetNewCustomer(ref customerTableset);
                                var customerRow = customerTableset.Customer.Where(p => p.RowMod == "A").FirstOrDefault();
                                if (customerRow != null)
                                {
                                    MapToRow(customerRow, customer);
                                    customerClient.Update(ref customerTableset);
                                    customer.DMSFlag = "S";
                                    Console.WriteLine($"Added customer: #{customer.CustomerCode} successfully!");
                                }
                            }
                            catch (Exception e)
                            {
                                customer.DMSFlag = "F";
                                Console.WriteLine($"Added customer: #{customer.CustomerCode} failed! - {e.Message}");
                                Console.WriteLine(e.GetBaseException().Message);
                                continue;
                            }
                        }

                        foreach(var customer in updatedCustomers)
                        {
                            try
                            {
                                CustomerTableset customerTableset = customerClient.GetByCustID(customer.CustomerCode, true);
                                var customerRow = customerTableset.Customer.FirstOrDefault();
                                if (customerRow != null)
                                {
                                    customerRow.RowMod = "U";
                                    MapToRow(customerRow, customer);
                                    customerClient.Update(ref customerTableset);
                                    customer.DMSFlag = "S";
                                    Console.WriteLine($"Updated customer: #{customer.CustomerCode} successfully!");
                                }
                            }
                            catch (Exception e)
                            {
                                customer.DMSFlag = "F";
                                Console.WriteLine($"Updated customer: #{customer.CustomerCode} failed! - {e.Message}");
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

        public async Task<int> GetCustNum(string custId)
        {
            var customer = await customerClient.GetCustomerAsync(custId);
            if (!customer.Customer.Any())
            {
                Console.WriteLine($"Customer {custId} not found!");
                return 0;
            }
            return customer.Customer[0].CustNum;
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
            row.Address1 = !string.IsNullOrEmpty(entity.Address) ? entity.Address : row.Address1;
            row.PhoneNum = !string.IsNullOrEmpty(entity.Phone1) ? entity.Phone1 : row.PhoneNum;
            row.FaxNum = !string.IsNullOrEmpty(entity.Fax) ? entity.Fax : row.FaxNum;
            row.CreditLimit = entity.CreditLimit.HasValue ? entity.CreditLimit.Value : row.CreditLimit;
            row.CurrencyCode = !string.IsNullOrEmpty(entity.Currency) ? entity.Currency : row.CurrencyCode;
            row.Country = !string.IsNullOrEmpty(entity.Country) ? entity.Country : row.Country;
            row.TermsCode = !string.IsNullOrEmpty(entity.PaymentTerm) ? entity.PaymentTerm : row.TermsCode;
            row.TaxRegionCode = !string.IsNullOrEmpty(entity.GroupTax) ? entity.GroupTax : row.TaxRegionCode;
            row.ResaleID = !string.IsNullOrEmpty(entity.TaxCode) ? entity.TaxCode : row.ResaleID;
            row.CustomerType = !string.IsNullOrEmpty(entity.CustomerType) ? entity.CustomerType : row.CustomerType;

            row.TerritoryID = "DEFAULT";
        }
    }
}
