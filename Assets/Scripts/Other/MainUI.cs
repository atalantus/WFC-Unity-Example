using LevelGeneration;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public LevelGenerator levelGenerator;
    public CameraController cameraController;

    private bool _isGenerating;

    public Text widthText;
    public Text heightText;

    public void Generate()
    {
        if (_isGenerating) return;
        _isGenerating = true;

        levelGenerator.GenerateLevel();

        cameraController.AdjustCamera(levelGenerator.width, levelGenerator.height);

        _isGenerating = false;
    }

    public void ChangeWidth(float width)
    {
        levelGenerator.width = Mathf.RoundToInt(width);
        widthText.text = width.ToString();
    }

    public void ChangeHeight(float height)
    {
        levelGenerator.height = Mathf.RoundToInt(height);
        heightText.text = height.ToString();
    }
}