using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity
{
    public interface IEntity
    {
        bool GetEntity();
        List<IEntity> GetEntities();
        List<IEntity> GetEntities(SQLCondition conditions);
        List<IEntity> GetPagedEntities(int pageIndex, int pageSize);
        List<IEntity> GetPagedEntities(int pageIndex, int pageSize, SQLCondition sqlCondition);

        bool Delete();
        bool Delete(SQLCondition sqlCondition);

        bool Insert();
        bool Update();
    }
}
