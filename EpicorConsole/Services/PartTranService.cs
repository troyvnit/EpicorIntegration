using EpicorConsole.Data;
using EpicorConsole.Epicor.PartTranSvc;
using EpicorConsole.Epicor.SessionModSvc;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicorConsole.Services
{
    public class PartTranService : BaseService
    {
        PartTranSvcContractClient partTranClient;
        SessionModSvcContractClient sessionModClient;

        public PartTranService()
        {
            var sessionModService = new SessionModService();
            var sessionId = sessionModService.Login();
            this.sessionId = sessionId;
            this.sessionModClient = sessionModService.sessionModClient;
            builder.Path = $"{environment}/Erp/BO/PartTran.svc";
            partTranClient = GetClient<PartTranSvcContractClient, PartTranSvcContract>(builder.Uri.ToString(), epicorUserID, epiorUserPassword, bindingType);
            partTranClient.Endpoint.EndpointBehaviors.Add(new HookServiceBehavior(sessionId, epicorUserID));
        }

        public async Task SyncPartTrans(string company)
        {
            log.Information("Syncing PartTrans...");
            Console.WriteLine($"Syncing {company} PartTrans...");
            try
            {
                using (var db = new EpicorIntergrationEntities())
                {
                    string siteID, siteName, workstationID, workstationDescription, employeeID, countryGroupCode, countryCode, tenantID, companyName, systemCode;
                    sessionModClient.SetCompany(company, out siteID, out siteName, out workstationID, out workstationDescription, out employeeID, out countryGroupCode, out countryCode, out tenantID);

                    bool more = true;
                    int page = 0;
                    DateTime expired = DateTime.Now.AddMinutes(10);
                    while (more && expired >= DateTime.Now)
                    {
                        page++;
                        Console.WriteLine($"Working on PartTran page: #{page}");
                        var rs = await partTranClient.GetRowsAsync(new Epicor.PartTranSvc.GetRowsRequest()
                        {
                            pageSize = 5,
                            absolutePage = page,
                            whereClausePartTran = "(TranType = 'STK-UKN' OR TranType = 'PUR-STK') AND TranDate >= '2017-9-1'"
                        });
                        var result = rs.GetRowsResult;
                        var partTrans = result.PartTran.ToArray();
                        more = rs.morePages;

                        foreach (var partTran in partTrans)
                        {
                            var invtTrans = db.INVT_TRANS.FirstOrDefault(p => p.TranNum == partTran.TranNum);
                            if (invtTrans == null)
                            {
                                invtTrans = new INVT_TRANS();
                                invtTrans.DMSFlag = "N";
                                MapToEntity(invtTrans, partTran);
                                db.INVT_TRANS.Add(invtTrans);
                                Console.WriteLine($"Added invt trans: #{partTran.TranNum}");
                            }
                            //else
                            //{
                            //    MapToEntity(invtTrans, partTran);
                            //    invtTrans.DMSFlag = "U";
                            //    db.INVT_TRANS.Attach(invtTrans);
                            //    db.Entry(invtTrans).State = System.Data.Entity.EntityState.Modified;
                            //    Console.WriteLine($"Updated invt trans: #{partTran.TranNum}");
                            //}
                        }
                    }
                    await db.SaveChangesAsync();
                }

                sessionModClient.Logout();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
            }
        }

        private void MapToEntity(INVT_TRANS entity, PartTranRow row)
        {
            entity.Company = row.Company;
            entity.InvtyUOM = row.InvtyUOM;
            entity.LotNum = row.LotNum;
            entity.OrderNum = row.OrderNum;
            entity.PartDescription = row.PartDescription;
            entity.PartNum = row.PartNum;
            entity.Site = row.Plant;
            entity.SysDate = row.SysDate;
            entity.TranClass = row.TranClass;
            entity.TranDate = row.TranDate;
            entity.TranNum = row.TranNum;
            entity.TranQty = row.TranQty;
            entity.TranType = row.TranType;
            entity.WareHouseCode = row.WareHouseCode;
            entity.CreatedBy = this.epicorUserID;
            entity.LastUpdatedBy = this.epicorUserID;
            entity.CreatedDateTime = DateTime.Now;
            entity.LastUpdatedDateTime = DateTime.Now;
        }
    }
}
