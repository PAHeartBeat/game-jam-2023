using UnityEngine;
using System.Collections.Generic;

public class TetrisShapeShooter : MonoBehaviour
{
    public GameObject cubePrefab;                // Prefab for the cube shape
    public Transform cannonTransform;            // Transform of the cannon

    public float shootInterval = 2f;             // Interval between each shape shoot
    public float shootForce = 10f;               // Force to apply to the shape when shooting
    public float bulletLifetime = 5f;            // Time in seconds before the bullet is destroyed
    public float recoilDistance = 3f;            // Distance the player is pushed back when the cannon fires
    public float moveSpeed = 5f;                  // Speed at which the player moves
    public ThirdPersonCamera thirdPersonCamera;           

    [SerializeField] private float smoothness = 0.5f;
    private float shootTimer = 0f;               // Timer for shooting shapes
    private bool canUseGun = true;               // Flag to indicate if the player can use the gun
    private int currentShapeIndex = 0;           // Index of the currently selected shape

    private List<Vector2Int[]> tetrisShapes = new List<Vector2Int[]>
    {
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(1, -2) }, // L shape
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(-1, -2) }, // Reverse L shape
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0) }, // Square shape
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, -1) }, // Z shape
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(-1, -1) }, // Reverse Z shape
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(0, -3) }, // Line shape
        new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(0, -2), new Vector2Int(-1, -2) }  // T shape
    };

    private void Update()
    {
        if (thirdPersonCamera.isCinematicView)
            return;
        // Update the shoot timer
        shootTimer += Time.deltaTime;

        // Check if it's time to shoot a shape
        if (shootTimer >= shootInterval)
        {
            // Shoot a random Tetris shape
            if (canUseGun)
            {
                ShootTetrisShape();
            }

            // Reset the shoot timer
            shootTimer = 0f;
        }

        // Move the player automatically
        float targetZ = transform.position.z + moveSpeed * Time.deltaTime;
        Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, targetZ);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness);


        // Check for input to change the currently selected shape
        if (Input.GetButtonDown("Fire1"))
        {
            SelectNextShape();
        }
    }

    private void SelectNextShape()
    {
        // Increase the current shape index
        currentShapeIndex++;

        // Wrap the index around if it exceeds the shape count
        if (currentShapeIndex >= tetrisShapes.Count)
        {
            currentShapeIndex = 0;
        }

        // Print the selected shape index
        Debug.Log("Selected Shape Index: " + currentShapeIndex);
    }

    private void ShootTetrisShape()
    {
        // Select a random Tetris shape from the list
        int randomIndex = Random.Range(0, tetrisShapes.Count);
        Vector2Int[] tetrisShape = tetrisShapes[randomIndex];

        // Create a parent object for the shape
        GameObject shapeParent = new GameObject("TetrisShape");

        Color color = GetRandomColor();
        // Generate the shape using cubes
        for (int i = 0; i < tetrisShape.Length; i++)
        {
            // Create a cube instance
            GameObject cube = Instantiate(cubePrefab, shapeParent.transform);
            cube.transform.localPosition = new Vector3(tetrisShape[i].x, -tetrisShape[i].y, 0f);

            // Apply a random color to the cube
            Renderer cubeRenderer = cube.transform.GetChild(0).GetComponent<Renderer>();
            cubeRenderer.material.color = color;
        }

        // Position and scale the shape at the cannon's position
        shapeParent.transform.position = cannonTransform.position;
        shapeParent.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Adjust the scale as needed

        // Add a Rigidbody component to the parent object
        Rigidbody shapeParentRigidbody = shapeParent.AddComponent<Rigidbody>();
        shapeParentRigidbody.useGravity = false;
        // Apply a force to shoot the shape
        shapeParentRigidbody.AddForce(cannonTransform.forward * shootForce, ForceMode.Impulse);

        // Attach a script to the shape parent to allow rotation
        shapeParent.AddComponent<TetrisShapeRotation>();

        // Destroy the bullet after a certain amount of time
        Destroy(shapeParent, bulletLifetime);

        // Apply recoil to the player
        //ApplyRecoil();
    }

    private Color GetRandomColor()
    {
        // Generate random RGB values
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        // Create and return the color
        return new Color(r, g, b);
    }

    private void ApplyRecoil()
    {
        // Calculate the recoil direction
        Vector3 recoilDirection = -cannonTransform.forward;

        // Push the player back
        transform.position += recoilDirection * recoilDistance;

        // Disable the gun usage temporarily
        canUseGun = false;

        // Start a coroutine to enable gun usage after a delay
        StartCoroutine(EnableGunAfterDelay(shootInterval));
    }

    private System.Collections.IEnumerator EnableGunAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canUseGun = true;
    }
}
