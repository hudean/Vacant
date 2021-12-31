using Vacant.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vacant.Domain
{
    public class AuditPropertySetter : IAuditPropertySetter
    {
        protected Guid? CurrentUserId { get; }
        protected Guid? CurrentTenantId { get; }
        protected IClock Clock { get; }

        public AuditPropertySetter(
            IClock clock)
        {
            //CurrentUserId = currentUserId;
            //CurrentTenantId = currentTenantId;
            Clock = clock;
        }


        /// <summary>
        /// 设置创建人属性
        /// </summary>
        /// <param name="targetObject"></param>
        public void SetCreationProperties(object targetObject)
        {
            SetCreationTime(targetObject);
            SetCreatorId(targetObject);
        }

        public void SetModificationProperties(object targetObject)
        {
            SetLastModificationTime(targetObject);
            SetLastModifierId(targetObject);
        }

        public void SetDeletionProperties(object targetObject)
        {
            SetDeletionTime(targetObject);
            SetDeleterId(targetObject);
        }

        private void SetCreationTime(object targetObject)
        {
            if (!(targetObject is IHasCreationTime objectWithCreationTime))
            {
                return;
            }

            if (objectWithCreationTime.CreationTime == default)
            {
                objectWithCreationTime.CreationTime = Clock.Now;
            }
        }

        private void SetCreatorId(object targetObject)
        {
            if (!(targetObject is ICreationAudited entity))
            {
                return;
            }
            if (entity.CreatorUserId != null)
            {
                return;
            }
            if (!CurrentUserId.HasValue)
            {
                return;
            }

            if (targetObject is IMultiTenant multiTenantEntity)
            {
                if (multiTenantEntity.TenantId != CurrentTenantId)
                {
                    return;
                }
            }

            entity.CreatorUserId = CurrentUserId;
        }

        private void SetLastModificationTime(object targetObject)
        {
            if (targetObject is IHasModificationTime objectWithModificationTime)
            {
                objectWithModificationTime.LastModificationTime = Clock.Now;
            }
        }

        private void SetLastModifierId(object targetObject)
        {
            if (!(targetObject is IModificationAudited modificationAudited))
            {
                return;
            }

            if (!CurrentUserId.HasValue)
            {
                modificationAudited.LastModifierUserId = null;
                return;
            }

            if (modificationAudited is IMultiTenant multiTenantEntity)
            {
                if (multiTenantEntity.TenantId != CurrentTenantId)
                {
                    modificationAudited.LastModifierUserId = null;
                    return;
                }
            }

            modificationAudited.LastModifierUserId = CurrentUserId;
        }

        private void SetDeletionTime(object targetObject)
        {
            if (targetObject is IHasDeletionTime objectWithDeletionTime)
            {
                if (objectWithDeletionTime.DeletionTime == null)
                {
                    objectWithDeletionTime.DeletionTime = Clock.Now;
                }
            }
        }

        private void SetDeleterId(object targetObject)
        {
            if (!(targetObject is IDeletionAudited deletionAudited))
            {
                return;
            }

            if (deletionAudited.DeleterUserId != null)
            {
                return;
            }

            if (CurrentUserId == null)
            {
                deletionAudited.DeleterUserId = null;
                return;
            }

            if (deletionAudited is IMultiTenant multiTenantEntity)
            {
                if (multiTenantEntity.TenantId != CurrentTenantId)
                {
                    deletionAudited.DeleterUserId = null;
                    return;
                }
            }

            deletionAudited.DeleterUserId = CurrentUserId;
        }
    }
}
