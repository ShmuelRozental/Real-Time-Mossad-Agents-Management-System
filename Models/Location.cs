using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Real_Time_Mossad_Agents_Management_System.Enums;

namespace Real_Time_Mossad_Agents_Management_System.Models

{

    [Owned]
    public class Location
    {
        [Range(0, 1000, ErrorMessage = "X must be between 0 and 1000.")]
        public int X { get; set; }

        [Range(0, 1000, ErrorMessage = "Y must be between 0 and 1000.")]
        public int Y { get; set; }

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.NW:
                    X -= 1;
                    Y -= 1;
                    break;
                case Direction.N:
                    Y -= 1;
                    break;
                case Direction.NE:
                    X += 1;
                    Y -= 1;
                    break;
                case Direction.W:
                    X -= 1;
                    break;
                case Direction.E:
                    X += 1;
                    break;
                case Direction.SW:
                    X -= 1;
                    Y += 1;
                    break;
                case Direction.S:
                    Y += 1;
                    break;
                case Direction.SE:
                    X += 1;
                    Y += 1;
                    break;
            }


            X = Math.Clamp(X, 0, 1000);
            Y = Math.Clamp(Y, 0, 1000);
        }
    }
}
