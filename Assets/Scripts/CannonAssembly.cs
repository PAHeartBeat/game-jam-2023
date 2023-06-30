using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonAssembly : MonoBehaviour
{
	public List<GameObject> loadedShapes = new List<GameObject>(); // List to store the loaded shapes

	public void LoadShape(GameObject shape) {
		// Do something with the loaded shape
		// Example: Add the shape to the list
		loadedShapes.Add(shape);
	}
}
