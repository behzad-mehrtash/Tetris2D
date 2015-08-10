﻿using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour {

	// Time since last gravity tick
	float lastFall = 0;

	public bool isValidGridPos() {        
		foreach (Transform child in transform) {
			Vector2 v = Grid.roundVec2(child.position);
			
			// Not inside Border?
			if (!Grid.insideBorder(v))
				return false;
			
			// Block in grid cell (and not part of same group)?
			if (Grid.grid[(int)v.x, (int)v.y] != null &&
			    Grid.grid[(int)v.x, (int)v.y].parent != transform)
				return false;
		}
		return true;
	}

	public void updateGrid() {
		// Remove old children from grid
		for (int y = 0; y < Grid.h; ++y)
			for (int x = 0; x < Grid.w; ++x)
				if (Grid.grid[x, y] != null)
					if (Grid.grid[x, y].parent == transform)
						Grid.grid[x, y] = null;
		
		// Add new children to grid
		foreach (Transform child in transform) {
			Vector2 v = Grid.roundVec2(child.position);
			Grid.grid[(int)v.x, (int)v.y] = child;
		}        
	}

	void Update() {
		// Move Left
		if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			// Modify position
			transform.position += new Vector3(-1, 0, 0);
			
			// See if valid
			if (isValidGridPos())
				// It's valid. Update grid.
				updateGrid();
			else
				// It's not valid. revert.
				transform.position += new Vector3(1, 0, 0);
		}
		
		// Move Right
		else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			// Modify position
			transform.position += new Vector3(1, 0, 0);
			
			// See if valid
			if (isValidGridPos())
				// It's valid. Update grid.
				updateGrid();
			else
				// It's not valid. revert.
				transform.position += new Vector3(-1, 0, 0);
		}
		
		// Rotate
		else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			transform.Rotate(0, 0, -90);
			
			// See if valid
			if (isValidGridPos())
				// It's valid. Update grid.
				updateGrid();
			else
				// It's not valid. revert.
				transform.Rotate(0, 0, 90);
		}
		
		// Move Downwards and Fall
		else if (Input.GetKeyDown(KeyCode.DownArrow) ||
		         Time.time - lastFall >= 1) {
			// Modify position
			transform.position += new Vector3(0, -1, 0);
			
			// See if valid
			if (isValidGridPos()) {
				// It's valid. Update grid.
				updateGrid();
			} else {
				// It's not valid. revert.
				transform.position += new Vector3(0, 1, 0);
				
				// Clear filled horizontal lines
				Grid.deleteFullRows();
				
				// Spawn next Group
				FindObjectOfType<Spawner>().spawnNext();
				
				// Disable script
				enabled = false;
			}
			
			lastFall = Time.time;
		}
	}


	// Use this for initialization
	void Start () {
		// Default position not valid? Then it's game over
		if (!isValidGridPos()) {
			Debug.Log("GAME OVER");
			Destroy(gameObject);
		}
	}
}
