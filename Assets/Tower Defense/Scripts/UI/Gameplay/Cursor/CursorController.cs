using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Patterns;

[RequireComponent(typeof(CursorManager))]
[HideMonoScript]
public class CursorController : Singleton<CursorController>
{
    // Private (Variables) [START]
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private CursorTypeEnum selectedCursor = CursorTypeEnum.ARROW_01;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private int currentFrame;
    [ShowInInspector]
    [HideInEditorMode]
    [ReadOnly]
    private float frameTimer;
    // Private (Variables) [END]

    // (Unity) Methods [START]
    private void Start()
    {
        InitializeVariables();
    }
    private void Update()
    {
        FilterCursorChange();
        HandleCursorUpdate();
    }
    // (Unity) Methods [END]


    // Public (Methods) [START]
    public void ResetCursorVariables()
    {
        currentFrame = 0;
        frameTimer = 0f;
    }
    // Public (Methods) [END]

    // Private (Methods) [START]
    private void FilterCursorChange()
    {
        bool didFilter = false;

        if (PlayerCommandsManager.instance.IsCasting)
        {
            switch (CursorManager.instance.Mode)
            {
                case CursorModeEnum.CASTING_FLAG:
                    didFilter = true;

                    HandleCursorFlagMode();
                    break;
                case CursorModeEnum.CASTING_AIM:
                    didFilter = true;

                    HandleCursorAimMode();
                    break;
            }
        }

        if (!didFilter)
            HandleCursorIdleMode();
    }
    private void HandleCursorAimMode()
    {
        if (Input.GetMouseButton(0))
            ChangeSelectedCursor(CursorTypeEnum.AIM_02);
        else
            ChangeSelectedCursor(CursorTypeEnum.AIM_01);

        if (Input.GetMouseButtonUp(0) && !OverlayInterfaceManager.instance.IsOverUI())
        {
            PlayerCommandsManager.instance.Command = PlayerCommandEnum.IDLE;
            CursorManager.instance.Mode = CursorModeEnum.IDLE;
            CursorManager.instance.aimCasting.Occurred(SelectionManager.instance.SelectedAgents.FirstOrDefault().gameObject);
        }
    }
    private void HandleCursorFlagMode()
    {
        if (Input.GetMouseButton(0))
            ChangeSelectedCursor(CursorTypeEnum.FLAG_02);
        else
            ChangeSelectedCursor(CursorTypeEnum.FLAG_01);

        if (Input.GetMouseButtonUp(0) && !OverlayInterfaceManager.instance.IsOverUI())
        {
            PlayerCommandsManager.instance.Command = PlayerCommandEnum.IDLE;
            CursorManager.instance.Mode = CursorModeEnum.IDLE;
            CursorManager.instance.flagPositioning.Occurred(SelectionManager.instance.SelectedAgents.FirstOrDefault().gameObject);
        }
    }
    private void HandleCursorIdleMode()
    {
        if (Input.GetMouseButton(0))
            ChangeSelectedCursor(CursorTypeEnum.ARROW_02);
        else
            ChangeSelectedCursor(CursorTypeEnum.ARROW_01);
    }
    private void HandleCursorUpdate()
    {
        CursorSO cursor = CursorManager.instance.GetCursorByType(selectedCursor);

        if (cursor.isAnimated)
        {
            frameTimer -= Time.deltaTime;

            if (frameTimer <= 0f)
            {
                frameTimer += cursor.animationVelocity;
                currentFrame = (currentFrame + 1) % cursor.textures.Count;
            }
        }

        Cursor.SetCursor(cursor.textures[currentFrame], cursor.offsets[currentFrame], CursorMode.Auto);
    }
    private void ChangeSelectedCursor(CursorTypeEnum newCursor)
    {
        if (selectedCursor == newCursor)
            return;

        selectedCursor = newCursor;
        ResetCursorVariables();        
    }
    private void InitializeVariables()
    {
        ResetCursorVariables();
    }
    // Private (Methods) [END]
}
