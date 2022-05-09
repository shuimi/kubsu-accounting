using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace accounting.Services
{
    public class PositionsService
    {
        ApplicationContext db;
        
        public PositionsService(ApplicationContext applicationContext)
        {
            db = applicationContext;
        }
        
        public bool isExist(Position candidate)
        {
            var position = db.Positions
                   .FirstOrDefault(item => item.Name == candidate.Name);

            if (position == null)
            {
                return false;
            }
            return true;
        }

        public int findIndexByName(string name)
        {
            var position = db.Positions
                   .FirstOrDefault(item => item.Name == name);

            if (position == null)
            {
                return -1;
            }

            return position.Id;
        }

        public List<Position> getPositionsList()
        {
            var positions = db.Positions.ToList();
            return positions;
        }

        public Position? getPositionById(int id)
        {
            var position = db.Positions
                   .FirstOrDefault(item => item.Id == id);
            
            return position;
        }

        public void addPosition(Position position)
        {
            db.Positions.Add(position); 
            db.SaveChanges();
        }

        public void updatePosition(int id, Position position)
        {
            var positionToUpdate = db.Positions
                .FirstOrDefault(item => item.Id == id);
            
            if (positionToUpdate != null) 
            {
                positionToUpdate.Name = position.Name;
                positionToUpdate.Description = position.Description;
            }

            db.SaveChanges();
        }

        public void deletePosition(int id)
        {
            db.Positions.Remove(getPositionById(id));
            db.SaveChanges();
        }
    }
}
