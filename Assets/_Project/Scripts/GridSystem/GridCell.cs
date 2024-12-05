using UnityEngine;

namespace SMTD.Grid
{
    public class GridCell
    {
        public GridCell(bool isWalkable=true)
        {
            IsWalkable = isWalkable;
        }
        public Vector3Int CellPosition { get; set; } // Grid'in cell pozisyonu (Grid koordinatları)
        public Vector3 WorldPosition { get; set; } // Gerçek dünya pozisyonu
        public bool IsWalkable { get; set; } // Hücreye geçilebilir mi?
        public GridCell Parent { get; set; } // Yolun geri izlenmesi için

        public int G { get; set; } // Başlangıçtan buraya maliyet
        public int H { get; set; } // Hedeften buraya tahmini maliyet
        public int F => G + H; // Toplam maliyet
    }
}