using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the creation and display of a rectangular on-screen box
/// used to select multiple Units within the game.
/// </summary>
/// <remarks>
/// This script captures mouse input to form a selection rectangle
/// and detects which units are inside that rectangle upon mouse release.
/// It relies on <see cref="UnitSelectionManager"/> to finalize the actual selection.
/// Attatch this component to a UI element with a <see cref="RectTransform"/> for the selection box visual.
/// </remarks>
public class UnitSelectionBox : MonoBehaviour
{
    /// <summary>
    /// A reference to the main camera used for converting screen coordinates
    /// (mouse position) to world coordinates.
    /// </summary>
    private Camera myCam;

    /// <summary>
    /// The UI RectTransform used to visualize the drag-selection box.
    /// </summary>
    [SerializeField] private RectTransform boxVisual;

    /// <summary>
    /// Stores the bounding rectangle of the current selection in screen space.
    /// </summary>
    Rect selectionBox;

    /// <summary>
    /// The position of the mouse in screen coordinates at the moment the left
    /// mouse button is pressed.
    /// </summary>
    Vector2 startPosition;

    /// <summary>
    /// The position of the mouse in screen coordinates while the left mouse 
    /// button is being held down, or when it is released.
    /// </summary>
    Vector2 endPosition;

    /// <summary>
    /// Unity's Start method, called before the first frame update.
    /// Initializes references and ensures the selection box is hidden at the start.
    /// </summary>
    private void Start()
    {
        // Get the main camera from the scene
        myCam = Camera.main;

        // Initialize the start and end positions to zero
        startPosition = Vector2.zero;
        endPosition = Vector2.zero;

        // Draw an "empty" box so it's hidden initially
        DrawVisual();
    }

    /// <summary>
    /// Unity's Update method, called once per frame.
    /// Tracks mouse input and updates the selection box accordingly.
    /// </summary>
    private void Update()
    {
        // When left mouse button is first pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Save the mouse position at click as the start of our selection
            startPosition = Input.mousePosition;

            // Reset the selectionBox boundaries
            selectionBox = new Rect();
        }

        // While holding down the left mouse button (dragging)
        if (Input.GetMouseButton(0))
        {
            if (boxVisual.rect.width > 0 || boxVisual.rect.height > 0)
            {
                UnitSelectionManager.Instance.DeselectAllUnits();
                // While dragging also select the units
                SelectUnits();
            }

            // Continuously update the end position as the mouse moves
            endPosition = Input.mousePosition;

            // Redraw the UI box to reflect the current drag area
            DrawVisual();

            // Update the actual selectionBox boundaries in screen space
            DrawSelection();
        }

        // When the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            // Perform final selection of all units within the box
            SelectUnits();

            // Reset start and end positions
            startPosition = Vector2.zero;
            endPosition = Vector2.zero;

            // Clear the UI box
            DrawVisual();
        }
    }

    /// <summary>
    /// Updates the visual <see cref="RectTransform"/> to match the current 
    /// drag area defined by startPosition and endPosition.
    /// </summary>
    private void DrawVisual()
    {
        // Calculate the two corners of the box
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;

        // Find the center by averaging the two corners
        Vector2 boxCenter = (boxStart + boxEnd) / 2f;

        // Move the box visual to the center
        boxVisual.position = boxCenter;

        // Determine the size by taking absolute differences
        Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        // Apply the size to the RectTransform to visually match the drag area
        boxVisual.sizeDelta = boxSize;
    }

    /// <summary>
    /// Calculates the boundaries of <see cref="selectionBox"/> based on 
    /// current mouse position versus the starting position.
    /// </summary>
    private void DrawSelection()
    {
        // Set xMin and xMax based on which side of startPosition.x we're dragging
        if (Input.mousePosition.x < startPosition.x)
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMin = startPosition.x;
            selectionBox.xMax = Input.mousePosition.x;
        }

        // Set yMin and yMax based on which side of startPosition.y we're dragging
        if (Input.mousePosition.y < startPosition.y)
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMin = startPosition.y;
            selectionBox.yMax = Input.mousePosition.y;
        }
    }

    /// <summary>
    /// Selects all units whose screen positions lie within the current 
    /// <see cref="selectionBox"/>, as tracked by <see cref="UnitSelectionManager"/>.
    /// </summary>
    private void SelectUnits()
    {
        // Loop through all units managed by our UnitSelectionManager
        foreach (var unit in UnitSelectionManager.Instance._allUnitsList)
        {
            // Convert the unit's world position to screen space and check if it's inside the selectionBox
            if (selectionBox.Contains(myCam.WorldToScreenPoint(unit.transform.position)))
            {
                // Instruct the UnitSelectionManager to select this unit
                UnitSelectionManager.Instance.DragSelect(unit);
            }
        }
    }
}
