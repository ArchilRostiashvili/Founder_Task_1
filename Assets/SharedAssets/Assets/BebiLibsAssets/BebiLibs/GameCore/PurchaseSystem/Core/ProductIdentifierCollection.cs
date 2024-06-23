using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BebiLibs.PurchaseSystem.Core
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ProductCollection", menuName = "BebiLibs/PurchaseSystem/ProductCollection", order = 0)]
    public class ProductIdentifierCollection : ScriptableObject, IList<ProductIdentifier>
    {
        [ObjectInspector(true)]
        [SerializeField] private List<ProductIdentifier> _productIdentifierList = new List<ProductIdentifier>();
        public List<ProductIdentifier> ProductList => _productIdentifierList;

        private void OnValidate()
        {
            RemoveDuplicates();
        }

        private void RemoveDuplicates()
        {
            for (int i = 0; i < _productIdentifierList.Count; i++)
            {
                for (int j = i + 1; j < _productIdentifierList.Count; j++)
                {
                    if (_productIdentifierList[i] == _productIdentifierList[j])
                    {
                        _productIdentifierList.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public ProductIdentifier this[int index]
        {
            get => _productIdentifierList[index];
            set => _productIdentifierList[index] = value;
        }

        public int Count => _productIdentifierList.Count;
        public bool IsReadOnly => false;
        public void Add(ProductIdentifier item) => _productIdentifierList.Add(item);
        public void Clear() => _productIdentifierList.Clear();
        public bool Contains(ProductIdentifier moduleGameKind) => _productIdentifierList.Contains(moduleGameKind);
        public void CopyTo(ProductIdentifier[] array, int arrayIndex) => _productIdentifierList.CopyTo(array, arrayIndex);
        public IEnumerator<ProductIdentifier> GetEnumerator() => _productIdentifierList.GetEnumerator();
        public int IndexOf(ProductIdentifier item) => _productIdentifierList.IndexOf(item);
        public void Insert(int index, ProductIdentifier item) => _productIdentifierList.Insert(index, item);
        public bool Remove(ProductIdentifier item) => _productIdentifierList.Remove(item);
        public void RemoveAt(int index) => _productIdentifierList.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => _productIdentifierList.GetEnumerator();
    }
}
