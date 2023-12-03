﻿using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffInfoDisplay : MonoBehaviour
{
    private class BuffInfoObj
    {
        public readonly int ID;
        public readonly GameObject Prefab;
        public float Time;
        public readonly UILabel TimeLabel;

        public BuffInfoObj(int id, GameObject prefab, UILabel timeLabel)
        {
            ID = id;
            Prefab = prefab;
            TimeLabel = timeLabel;
        }
    }

    private Player player;
    private BuffInformationList buffs;
    private readonly List<BuffInfoObj> currentActiveBuffs = new List<BuffInfoObj>();
    private readonly List<BuffInfoObj> availableBuffs = new List<BuffInfoObj>();
    private UIGrid displayGrid;
    private int maxDisplayableBuffs; // not used

    private void Awake()
    {
        displayGrid = gameObject.GetComponent<UIGrid>();
        maxDisplayableBuffs = displayGrid.maxPerLine;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        buffs = GameManager.Instance.Buffs;
        var count = buffs.Count;

        for (var i = 0; i < count; ++i)
        {
            var buff = buffs[i];

            var prefab = Instantiate(buff.indicatorPrefab, gameObject.transform, false);
            prefab.SetActive(false);

            availableBuffs.Add(new BuffInfoObj(i, prefab, prefab.GetComponentInChildren<UILabel>()));
        }
    }

    private void FixedUpdate()
    {
        var listSize = currentActiveBuffs.Count;

        if (listSize == 0) return;
        if (player.IsDead)
        {
            foreach (var buffInfoObj in currentActiveBuffs)
            {
                buffInfoObj.Prefab.SetActive(false);
            }
            currentActiveBuffs.Clear();

            return;
        }
            
        foreach (var currentActiveBuff in currentActiveBuffs)
        {
            currentActiveBuff.Time = Mathf.Max(currentActiveBuff.Time - Time.deltaTime, 0f);
            currentActiveBuff.TimeLabel.text = (currentActiveBuff.Time > 1f) ? currentActiveBuff.Time.ToString("0") : currentActiveBuff.Time.ToString("0.0");
        }
    }

    public void AddBuffToDisplay(int buffIdentifier, float effectTime)
    {
        var alreadyActiveIndex = currentActiveBuffs.FindIndex(element => element.ID == buffIdentifier);
        if (alreadyActiveIndex != -1)
        {
            currentActiveBuffs[alreadyActiveIndex].Time = effectTime;
            currentActiveBuffs[alreadyActiveIndex].TimeLabel.text = effectTime.ToString();
        }
        else
        {
            var notYetActiveIndex = availableBuffs.FindIndex(element => element.ID == buffIdentifier);
            var buffInfoObj = availableBuffs[notYetActiveIndex];
            buffInfoObj.Time = effectTime;
            buffInfoObj.TimeLabel.text = effectTime.ToString();
            buffInfoObj.Prefab.SetActive(true);
            currentActiveBuffs.Add(buffInfoObj);
            displayGrid.Reposition();
        }
    }

    public void RemoveDisplayingBuff(int buffIdentifier)
    {
        foreach (var buff in currentActiveBuffs.FindAll(element => element.ID == buffIdentifier))
        {
            buff.Prefab.SetActive(false);
            currentActiveBuffs.Remove(buff);
        }

        displayGrid.Reposition();
    }

    public void RemoveAllDisplayingBuffs()
    {
        var listSize = currentActiveBuffs.Count;

        for (var i = listSize - 1; i >= 0; --i)
        {
            currentActiveBuffs[i].Prefab.SetActive(false);
            currentActiveBuffs.RemoveAt(i);
        }
    }
}