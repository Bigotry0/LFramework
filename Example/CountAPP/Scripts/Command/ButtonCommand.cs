using UnityEngine;

namespace LFramework.AI.Example.CountAPP
{
    /// <summary>
    /// Command类，需继承AbstractCommand抽象类，实现其OnExecute抽象方法
    /// Command中可定义字段，用于发送命令时进行数据传递（如有必要）
    /// </summary>
    public class AddButtonCommand : AbstractCommand
    {
        public override void OnExecute()
        {
            //获取模块并更改模块内数值
            this.GetModel<ICountModel>().Count.Value++;
            Debug.Log(this.GetModel<ICountModel>().Count.Value);
        }
    }

    public class DesButtonCommand : AbstractCommand
    {
        public override void OnExecute()
        {
            this.GetModel<ICountModel>().Count.Value--;
            Debug.Log(this.GetModel<ICountModel>().Count.Value);
        }
    }
}