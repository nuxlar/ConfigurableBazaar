using UnityEngine;
using System.Collections.Generic;

namespace ConfigurableBazaar
{
    internal class Printer
    {
        public Vector3 position;
        public Vector3 rotation;

        public Printer(Vector3 position, Vector3 rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
}
