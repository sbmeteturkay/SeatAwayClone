using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SMTD.BusPassengerGame;
using UnityEditor;
using UnityEngine;

namespace SMTD.Grid.Level
{
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/LevelData")]
    public class LevelData : SerializedScriptableObject
    {
        public LevelDesignData levelDesignData;
        [MinValue(1), OnValueChanged("GenerateGrid")]
        public Vector2Int GridSize; // Grid'in yatay boyutu
        public Vector2 CellSize; // Grid'in yatay boyutu
        public List<DefinedColors> passengers = new ();
        
        [TableMatrix(DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false)]
        public Cell[,] Grid;

        [Button("Generate Grid")]
        public void GenerateGrid()
        {
            Grid = new Cell[GridSize.x, GridSize.y];
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    Grid[x, y] = new Cell();
                }
            }
        }
#if UNITY_EDITOR

        private static Cell DrawColoredEnumElement(Rect rect, Cell cell)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                if (cell.DefinedColors == null)
                {
                    cell.DefinedColors = 0;
                }
                else if ((int)cell.DefinedColors+1 < Enum.GetValues(typeof(DefinedColors)).Length)
                {
                    cell.DefinedColors += 1;
                }
                else
                {
                    cell.DefinedColors = null;
                }

                
                GUI.changed = true;
                Event.current.Use();
            }

            EditorGUI.DrawRect(rect.Padding(1), cell.CellColor);

            return cell;
        }

#endif
    }
    [Serializable]
    public class Cell
    {
        public DefinedColors? DefinedColors;

        [ShowInInspector, ReadOnly]
        public Color CellColor
        {
            get
            {
                if (DefinedColors == null)
                {
                    return Color.white;
                }
                return DefinedColors switch
                {
                    BusPassengerGame.DefinedColors.Blue => Color.blue,
                    BusPassengerGame.DefinedColors.Orange => Color.yellow,
                    BusPassengerGame.DefinedColors.Red => Color.red,
                    _ => Color.white
                };
            }
        }
    }

}
