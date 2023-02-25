using System.Collections.Generic;
using UnityEngine;

namespace LFramework.AI.BehaviorTree
{
    /// <summary>
    /// 节点执行状态
    /// </summary>
    public enum BTResult
    {
        Ended = 1,
        Running = 2
    }
    
    /// <summary>
    /// 节点基类
    /// </summary>
    public class BTNode
    {
        //节点名称
        public string Name;

        //子节点列表
        protected List<BTNode> _children;
        
        //节点属性
        public List<BTNode> Children => _children;

        //节点准入条件
        public BTPrecondition Precondition;

        //数据库（Blackboard）
        public BTDatabase Database;

        //间隔
        public float InterVal = 0;

        //最后时间评估
        private float _lastTimeEvaluated = 0;

        //是否激活
        public bool Activated;
        
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="precondition">准入条件</param>
        public BTNode(BTPrecondition precondition)
        {
            this.Precondition = precondition;
        }        
        
        public BTNode () : this (null) {}

        /// <summary>
        /// 激活数据库
        /// </summary>
        /// <param name="database">数据库</param>
        public virtual void Activate(BTDatabase database)
        {
            if(Activated) return;

            this.Database = database;

            //TODO: Init
            if (Precondition != null)
            {
                Precondition.Activate(database);
            }

            if (_children != null)
            {
                foreach (var child in _children)
                {
                    child.Activate(database);
                }
            }
        }

        public bool Evaluate()
        {
            bool coolDownOK = CheckTimer();

            return Activated && coolDownOK && (Precondition == null || Precondition.Check()) && DoEvaluate();
        }

        protected virtual bool DoEvaluate()
        {
            return true;
        }

        public virtual BTResult Tick()
        {
            return BTResult.Ended;
        }
        
        public virtual void Clear() {}

        public virtual void AddChild(BTNode node)
        {
            if (_children == null)
            {
                _children = new List<BTNode>();
            }

            if (node != null)
            {
                _children.Add(node);
            }
        }

        public virtual void RemoveChild(BTNode node)
        {
            if (_children != null && node != null)
            {
                _children.Remove(node);
            }
        }

        /// <summary>
        /// 检查冷却时间是否结束
        /// </summary>
        /// <returns></returns>
        private bool CheckTimer()
        {
            if (Time.time - _lastTimeEvaluated > InterVal)
            {
                _lastTimeEvaluated = Time.time;
                return true;
            }

            return false;
        }
    }
}