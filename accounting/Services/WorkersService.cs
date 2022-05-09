using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace accounting.Services
{
    public class WorkersService
    {
        ApplicationContext db;
        
        public WorkersService(ApplicationContext applicationContext)
        {
            db = applicationContext;
        }

        public void addWorker(Worker worker)
        {
            db.Workers.Add(worker);
            db.SaveChanges();
        }

        public List<Worker> getWorkersList()
        {
            return db.Workers
                .Include(worker => worker.Positions)
                .ToList();
        }

        public Worker? getWorkerById(int id)
        {
            return db.Workers
                .Include(worker => worker.Positions)
                .ToList().Find(x => x.Id == id);
        }

        public void updateWorker(int id, Worker worker)
        {
            var workerToUpdate = db.Workers
                .FirstOrDefault(item => item.Id == id);

            if (workerToUpdate != null)
            {
                workerToUpdate.Firstname = worker.Firstname;
                workerToUpdate.Lastname = worker.Lastname;
                workerToUpdate.Middlename = worker.Middlename;
                workerToUpdate.EmploymentDate = worker.EmploymentDate;
                workerToUpdate.ContractId = worker.ContractId;
                workerToUpdate.Status = worker.Status;
                workerToUpdate.Positions = worker.Positions;
            }

            db.SaveChanges();
        }

        public void deleteWorker(int id)
        {
            db.Workers.Remove(getWorkerById(id));
            db.SaveChanges();
        }
    }
}
