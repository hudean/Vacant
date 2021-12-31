using System;
using Xunit;
using System.Linq;
using Vacant.EventBus;

namespace EventBus.Tests
{
    public class InMemory_SubscriptionManager_Tests
    {
        /// <summary>
        /// 创建后应为空
        /// </summary>
        [Fact]
        public void After_Creation_Should_Be_Empty()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            Assert.True(manager.IsEmpty);
        }

        /// <summary>
        /// 一个事件订阅后应包含该事件
        /// </summary>
        [Fact]
        public void After_One_Event_Subscription_Should_Contain_The_Event()
        {
            var manager = new InMemoryEventBusSubscriptionsManager();
            manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
            Assert.True(manager.HasSubscriptionsForEvent<TestIntegrationEvent>());
        }

        /// <summary>
        /// 删除所有订阅后事件不应再存在
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
        /// 删除最后一个订阅应该引发已删除的事件
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
        /// 获取事件处理程序应返回所有处理程序
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
