using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public GameManager GameManager;

    public Button startBtn;
    public Button resetBtn;
    public Dropdown touchTypeDropDown;
    public Dropdown pathFindingTypeDropDown;

    private void Awake() {
        startBtn.onClick.AddListener(() => {
            GameManager.nodeManager.StartFind();
        });
        resetBtn.onClick.AddListener(() => {
            GameManager.nodeManager.ResetFind();
        });
        touchTypeDropDown.onValueChanged.AddListener(value => {
            GameManager.nodeManager.ChangeUIPencilNode(value);
        });
        pathFindingTypeDropDown.onValueChanged.AddListener(value => {
            GameManager.nodeManager.ChangePathFindingType(value);
        });
    }
}
