using System.Collections.Generic;
using UnityEngine;

namespace LFramework.AI.BehaviorTree
{
    public class BTDatabase : MonoBehaviour
    {
        private List<object> _database = new List<object>();
        private List<string> _dataName = new List<string>();

        public T GetData<T>(string dataName) where T : class
        {
            int dataId = IndexOfDataId(dataName);
            if (dataId == -1)
            {
                Debug.LogError("Database: Data for " + dataName + " does not exist!");
            }

            return _database[dataId] as T;
        }

        public T GetData<T>(int dataId) where T : class
        {
            if (dataId >= _database.Count)
            {
                Debug.LogError("Database: Data for index" + dataId + " out of range!");
            }

            return _database[dataId] as T;
        }
        
        public int GetDataId(string dataName)
        {
            int dataId = IndexOfDataId(dataName);
            if (dataId == -1)
            {
                _dataName.Add(dataName);
                _database.Add(null);
                dataId = _database.Count - 1;
            }

            return dataId;
        }

        public void SetData<T>(string dataName, T data)
        {
            int dataId = GetDataId(dataName);
            _database[dataId] = data;
        }

        public void SetData<T>(int dataId, T data)
        {
            if (dataId >= _database.Count)
            {
                Debug.LogError("Database: dataId " + dataId + "is out of Range");
                return;
            }

            _database[dataId] = data;

        }

        private int IndexOfDataId(string dataName)
        {
            for (int i = 0; i < _dataName.Count; i++)
            {
                if (_dataName[i].Equals(dataName))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool ContainsData(string dataName)
        {
            return IndexOfDataId(dataName) != -1;
        }
    }
}