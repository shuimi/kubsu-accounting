using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accounting.Services
{
    public class AdvanceReportsService
    {
        
        ApplicationContext db;

        public AdvanceReportsService(ApplicationContext applicationContext)
        {
            db = applicationContext;
        }

        public List<AdvanceReport> getAdvanceReportsList()
        {
            var reports = db.AdvanceReports
                .Include(r => r.Head)
                .Include(r => r.AccountablePerson)
                .Include(r => r.Accountant)
                .Include(r => r.ChiefAccountant)
                .Include(r => r.AccountingRecords)
                .ToList();
            return reports;
        }

        public AdvanceReport? getAdvanceReportById(int id)
        {
            var report = db.AdvanceReports
                    .Include(r => r.Head)
                    .Include(r => r.AccountablePerson)
                    .Include(r => r.Accountant)
                    .Include(r => r.ChiefAccountant)
                    .Include(r => r.AccountingRecords)
                        .FirstOrDefault(item => item.Id == id);

            return report;
        }

        public void addAdvanceReport(AdvanceReport report)
        {
            db.AdvanceReports.Add(report);
            db.SaveChanges();
        }

        public void updateAdvanceReport(int id, AdvanceReport report)
        {
            var reportToUpdate = db.AdvanceReports
                .FirstOrDefault(item => item.Id == id);

            if (reportToUpdate != null)
            {
                reportToUpdate.Accountant = report.Accountant;
                reportToUpdate.ChiefAccountant = report.ChiefAccountant;
                reportToUpdate.AccountingRecords = report.AccountingRecords;
                reportToUpdate.Appointment = report.Appointment;
                reportToUpdate.AccountablePerson = report.AccountablePerson;
                reportToUpdate.Date = report.Date;
                reportToUpdate.Head = report.Head;
            }

            db.SaveChanges();
        }

        public void deleteAdvanceReport(int id)
        {
            db.AdvanceReports.Remove(getAdvanceReportById(id));
            db.SaveChanges();
        }

        public AccountingRecord getAccountingRecordById(int id)
        {
            var record = db.AccountingRecords
                   .FirstOrDefault(item => item.Id == id);

            return record;
        }

        public List<AccountingRecord> getAccountingRecordsList()
        {
            return db.AccountingRecords.ToList();
        }
    }
}
