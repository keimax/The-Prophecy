using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheProphecy
{
    public class ObjectPool 
    {
        private GameObject _prefab;
        private GameObject _container;

        private Stack<GameObject> _objectPool = new Stack<GameObject>();

        public ObjectPool(GameObject prefab, GameObject container)
        {
            _prefab = prefab;
            _container = container;
            Debug.Log(_prefab);
            Debug.Log(_container);
        }

        public void FillThePool(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject object_ = Object.Instantiate(_prefab);
                object_.transform.parent = _container.transform;
                AddToPool(object_);
                if (object_.GetComponent<Bullets>() != null) object_.GetComponent<Bullets>().InitializePool(this);
            }
        }

        public GameObject GetFromPool()
        {
            GameObject object_;
            if (_objectPool.Count > 0)
            {
                object_ = _objectPool.Pop();
                object_.gameObject.SetActive(true);

                return object_;
            }

            object_ = Object.Instantiate(_prefab);
            object_.transform.parent = _container.transform;
            return object_;
        }

        public void AddToPool(GameObject object_)
        {
            if (object_.GetComponent<Bullets>() != null) object_.GetComponent<Bullets>().InitializePool(this);
        //    Debug.Log(object_.gameObject.activeSelf);
            object_.gameObject.SetActive(false);
            _objectPool.Push(object_);
        //    Debug.Log(object_.gameObject.activeSelf);

        }
    }
}
