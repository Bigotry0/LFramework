namespace LFramework.AI.Example.EventExample
{
    public class SendEventCommand : AbstractCommand
    {
        public override void OnExecute()
        {
            //以下均为发送事件
            
            //直接传入泛型参数
            this.SendEvent<EventA>();
            
            //直接new()出事件实例
            this.SendEvent(new EventB()
            {
                a = 1,
                b = 0.1f
            });
            
            //发送分组事件，除了在泛型参数中传入分组接口，还应传入事件实例
            this.SendEvent<AGroup>(new EventC());
            
            this.SendEvent<AGroup>(new EventD());
        }
    }
}