using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Extensions;
using Microsoft.Xrm.Sdk.Client;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace APX.Xrm
{
    public static class ExtensionMethods
    {

        public static IEnumerable<T> GetRelatedEntities<T>(this Entity record, IOrganizationService service, string relationshipName) where T : Entity
        {
            var type = typeof(T);
            var entityName = type.GetCustomAttributes<EntityLogicalNameAttribute>(true).FirstOrDefault();

            //the related entity we are going to retrieve
            QueryExpression query = new QueryExpression();
            query.EntityName = entityName.LogicalName;
            query.ColumnSet = new ColumnSet();

            //the relationship that links the primary to the target
            Relationship relationship = new Relationship(relationshipName.ToLower());
            relationship.PrimaryEntityRole = EntityRole.Referenced; //important if the relationship is self-referencing

            //the query collection which forms the request
            RelationshipQueryCollection relatedEntity = new RelationshipQueryCollection();
            relatedEntity.Add(relationship, query);

            //the request to get the primary entity with the related records
            RetrieveRequest request = new RetrieveRequest();
            request.RelatedEntitiesQuery = relatedEntity;
            request.ColumnSet = new ColumnSet();
            request.Target = new EntityReference(record.LogicalName, record.Id);

            RetrieveResponse r = (RetrieveResponse)service.Execute(request);
            List<T> records = new List<T>();
            //query the returned collection for the target entity ids
            foreach(var result in r.Entity.RelatedEntities[relationship].Entities)
            {
                records.Add(result.ToEntity<T>());
            }
            
            return records;
        }
       

        public static T GetEntity<T>(this IOrganizationService serviceProxy, Guid id, ColumnSet columnSet)  where T : Entity
        {
            var type = typeof(T);
            var entityName = type.GetCustomAttributes<EntityLogicalNameAttribute>(true).FirstOrDefault();
            var entity = serviceProxy.Retrieve(entityName.LogicalName, id, columnSet).ToEntity<T>();
            return entity;
        }  
        public static T GetEntity<T>(this IOrganizationService serviceProxy, EntityReference record, ColumnSet columnSet)  where T : Entity
        {
       
            var entity = serviceProxy.Retrieve(record.LogicalName, record.Id, columnSet).ToEntity<T>();
            return entity;
        } 
        public static T GetRelated<T>(this IOrganizationService serviceProxy, EntityReference record, ColumnSet columnSet)  where T : Entity
        {
       
            var entity = serviceProxy.Retrieve(record.LogicalName, record.Id, columnSet).ToEntity<T>();
            return entity;
        }
        
        public static string GetPrimaryNameValue(this EntityReference entityRef, IOrganizationService orgService)
        {
            // Special case if Id is Guid.Empty
            if (entityRef.Id == Guid.Empty) return "Guid.Empty";

            // Populate result
            var result = "Null Entity";
            if (string.IsNullOrEmpty(entityRef.Name))
            {
                // retrieve record
                EntityMetadata entityMeta = orgService.GetEntityMetadata(entityRef.LogicalName); ;
                Entity e = orgService.Retrieve(entityRef.LogicalName, entityRef.Id, new ColumnSet(entityMeta.PrimaryNameAttribute));

                if (e != null)
                {
                    result = e.GetAttributeValue<string>(entityMeta.PrimaryNameAttribute);
                    if (string.IsNullOrEmpty(result))
                    {
                        result = (entityMeta.PrimaryNameAttribute + " is empty");
                    }
                    else
                    {
                        entityRef.Name = result;
                    }
                }
            }
            else
            {
                return entityRef.Name;
            }

            return result;
        }

    }
}
