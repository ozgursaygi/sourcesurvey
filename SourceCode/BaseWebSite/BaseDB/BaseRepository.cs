using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data;
using System.Data.Common;
using System.Data.Metadata.Edm;

namespace BaseDB
{
    public class BaseRepository<T> : IRepository 
            where T : ObjectContext, new()
    {
        protected T db = new T();

        public void Kaydet()
        {
            UpdateRecordTrackingMark();
           
            db.SaveChanges();
        }
        protected void UpdateRecordTrackingMark()
        {
            //Guid user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
            Guid user_uid = (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null) ? Guid.Empty : BaseDB.SessionContext.Current.ActiveUser.UserUid;
            
            var entries = from e in db.ObjectStateManager.GetObjectStateEntries(
                EntityState.Added | EntityState.Modified)
                          where e.Entity != null
                          select e;

            foreach (var entry in entries)
            {
                var fieldMetaData = entry.CurrentValues.DataRecordInfo.FieldMetadata;
                FieldMetadata updatedAtField = fieldMetaData
                    .Where(f => f.FieldType.Name == "updated_at").FirstOrDefault();
                FieldMetadata updatedByField = fieldMetaData
                    .Where(f => f.FieldType.Name == "updated_by").FirstOrDefault();


                FieldMetadata insertedAtField = fieldMetaData
                    .Where(f => f.FieldType.Name == "inserted_at").FirstOrDefault();
                FieldMetadata insertedByField = fieldMetaData
                    .Where(f => f.FieldType.Name == "inserted_by").FirstOrDefault();


                if (entry.State == EntityState.Added)
                {
                    if (insertedAtField.FieldType != null)
                        if (insertedAtField.FieldType.TypeUsage.EdmType.Name == PrimitiveTypeKind.DateTime.ToString())
                            entry.CurrentValues.SetDateTime(insertedAtField.Ordinal, DateTime.Now);

                    if (insertedByField.FieldType != null)
                        if (insertedByField.FieldType.TypeUsage.EdmType.Name == PrimitiveTypeKind.Guid.ToString())
                            entry.CurrentValues.SetGuid(insertedByField.Ordinal, user_uid);

                }
                if (entry.State == EntityState.Modified)
                {
                    if (updatedAtField.FieldType != null)
                        if (updatedAtField.FieldType.TypeUsage.EdmType.Name == PrimitiveTypeKind.DateTime.ToString())
                            entry.CurrentValues.SetDateTime(updatedAtField.Ordinal, DateTime.Now);

                    if (updatedByField.FieldType != null)

                        if (updatedByField.FieldType.TypeUsage.EdmType.Name == PrimitiveTypeKind.Guid.ToString())
                            entry.CurrentValues.SetGuid(updatedByField.Ordinal, user_uid);
                }
            }
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
