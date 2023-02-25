namespace LFramework.AI.Example.EventExample
{
    //!使用class或struct创建事件皆可，使用struct性能更高，推荐使用。
    //以下均为创建事件示例
    public struct EventA
    {
        //无参事件
    }

    public class EventB
    {
        //带参事件
        public int a;
        public float b;
    }

    //给事件分组
    public interface AGroup
    {
        
    }

    //继承接口即可分组
    public class EventC : AGroup
    {
        
    }
    public struct EventD : AGroup
    {
        
    }
}