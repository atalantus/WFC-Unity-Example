using System;
using System.Collections;
using System.Collections.Generic;
using LevelGeneration;
using UnityEngine;
using UnityEngine.UI;
using Grid = LevelGeneration.Grid;

public class MainUI : MonoBehaviour
{
    public Grid grid;
    public CameraController cameraController;
    private bool isGenerating;

    public Text widthText;
    public Text heightText;

    public void Generate()
    {
        if (isGenerating) return;
        isGenerating = true;

        grid.RemoveGrid();
        grid.GenerateGrid();
        LevelGenerator.Instance.GenerateLevelWFC(ref grid.cells, grid.seed != -1 ? grid.seed : Environment.TickCount);
        
        cameraController.AdjustCamera(grid.width, grid.height);
        
        var valid = grid.CheckGrid();
        if (!valid) Debug.LogError("Invalid Level!");

        isGenerating = false;
    }

    public void ChangeWidth(float width)
    {
        grid.width = Mathf.RoundToInt(width);
        widthText.text = width.ToString();
    }

    public void ChangeHeight(float height)
    {
        grid.height = Mathf.RoundToInt(height);
        heightText.text = height.ToString();
    }

    /*
    public void CheckLevel()
    {
        var valid = grid.CheckGrid();

        if (!valid) Debug.LogError("Invalid Level!");
        else Debug.Log("Valid Level!");
    }
    */
}