using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SyncObjectManager
{
    public class Manager
    {
        private static Manager _instance;
        public Dictionary<string, GameObject> ConnectedObjects { get; set; }

        public int NumberConnectedObjects { get; private set; }

        public string PlayerID { get; private set; }

        private Manager()
        {
            if (_instance != null)
            {
                return;
            }

            ConnectedObjects = new Dictionary<string, GameObject>();
            NumberConnectedObjects = 0;

            _instance = this;
        }

        public static Manager Instance
        {
            get
            {
                if (_instance == null)
                {
                    new Manager();
                }

                return _instance;
            }
        }

        public void AddObjectToConnectedObjects(string _objectID, GameObject _gameObject)
        {
            if (!ConnectedObjects.ContainsKey(_objectID))
            {
                ConnectedObjects.Add(_objectID, _gameObject);
                NumberConnectedObjects++;
            }
        }

        public void RemoveObjectsFromConnectedObjects(string _objectID)
        {
            if (ConnectedObjects.ContainsKey(_objectID))
            {
                ConnectedObjects.Remove(_objectID);
                NumberConnectedObjects--;
            }
        }

        public GameObject[] GetConnectedObjects()
        {
            return ConnectedObjects.Values.ToArray();
        }

        public void SetLocalPlayerID(string _playerID)
        {
            PlayerID = _playerID;
        }

        public GameObject GetObjectFromConnectedObjects(string _objectID)
        {
            if (ConnectedObjects.ContainsKey(_objectID))
            {
                return ConnectedObjects[_objectID];
            }

            return null;
        }
    }
}
