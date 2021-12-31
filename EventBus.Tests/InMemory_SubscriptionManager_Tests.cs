using System;
using Xunit;
using System.Linq;
using Vacant.EventBus;

namespace EventBus.Tests
{
    public class InMemory_SubscriptionManager_Tests
    {
        /// <summary>
        /// ������ӦΪ��
        /// </summary>
        [Fact]
        public void After_Creation_Should_Be_Empty()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            Assert.True(manager.IsEmpty);
        }

        /// <summary>
        /// һ���¼����ĺ�Ӧ�������¼�
        /// </summary>
        [Fact]
        public void After_One_Event_Subscription_Should_Contain_The_Event()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        /// <summary>
        /// ɾ�����ж��ĺ��¼���Ӧ�ٴ���
        /// </summary>
        [Fact]
        public void After_All_Subscriptions_Are_Deleted_Event_Should_No_Longer_Exists()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.False(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        /// <summary>
        /// ɾ�����һ������Ӧ��������ɾ�����¼�
        /// </summary>
        [Fact]
        public void Deleting_Last_Subscription_Should_Raise_On_Deleted_Event()
        {
            bool raised = false;
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.OnEventRemoved += (o, e) => raised = true;
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(raised);
        }

        /// <summary>
        /// ��ȡ�¼��������Ӧ�������д������
        /// </summary>
        [Fact]
        public void Get_Handlers_For_Event_Should_Return_All_Handlers()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationOtherEventHandler>();
            var handlers = manager.GetHandlersForEvent<TestIntegrationEvent>();
            Assert.Equal(2, handlers.Count());
        }
    }
}
