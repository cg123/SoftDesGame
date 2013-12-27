using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Dungeon
{
    public class GenerationOptions : Dictionary<string, int>
    {
        static Dictionary<string, int> defaults = new Dictionary<string, int>()
            {
                {"numRooms", 12},
                {"numExtraCorridors", 8},
                {"numItems", 7},
                {"numQuests", 3},
                {"roomWidthMin", 3},
                {"roomWidthMax", 10},
                {"roomHeightMin", 3},
                {"roomHeightMax", 10},
                {"corridorLengthMin", 3},
                {"corridorLengthMax", 10},
                {"placementTries", 10}
            };

        public GenerationOptions()
            : base(defaults)
        {
        }
        public GenerationOptions(IDictionary<string, int> values)
            : base(defaults)
        {
            foreach (KeyValuePair<string, int> pair in values)
            {
                this[pair.Key] = pair.Value;
            }
        }
    }
    public enum TileType
    {
        EMPTY = 0,
        WALL = 1,
        FLOOR = 2,
        TUNNEL = 3,
        ITEM = 4,
        QUEST = 5,
        START = 6
    }
    public class Room
    {
        public Room()
        {
            x0 = 0;
            y0 = 0;
            width = 0;
            height = 0;
        }
        public Room(int x0, int y0, int width, int height)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.width = width;
            this.height = height;
        }
        public static Room FromCorners(int x0, int y0, int x1, int y1)
        {
            int rx0, ry0;
            int width, height;
            width = Mathf.Abs(x1 - x0) + 1;
            height = Mathf.Abs(y1 - y0) + 1;
            rx0 = Mathf.Min(x0, x1);
            ry0 = Mathf.Min(y0, y1);
            return new Room(rx0, ry0, width, height);
        }
        public int x0, y0;
        public int width, height;

        public bool Intersects(Room other)
        {
            return !(
                x0 + width - 1 < other.x0 || x0 > other.x0 + other.width - 1 ||
                y0 + height - 1 < other.y0 || y0 > other.y0 + other.height - 1
                );
        }

        public int x1
        {
            get
            {
                return x0 + width - 1;
            }
        }
        public int y1
        {
            get
            {
                return y0 + height - 1;
            }
        }
    }
    public class Hallway : Room
    {
        public Hallway(int x0, int y0, int direction, int length)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.direction = direction;
            this.length = length;
            if (unitVectors[direction].x < 0)
            {
                this.x0 = x0 - width + 1;
                this.direction = (this.direction + 2) % 4;
            }
            else if (unitVectors[direction].y < 0)
            {
                this.y0 = y0 - height + 1;
                this.direction = (this.direction + 2) % 4;
            }
        }
        int _direction;
        public int direction
        {
            get
            {
                return _direction;
            }
            set
            {
                if (value < 0 || value > unitVectors.Length)
                {
                    throw new KeyNotFoundException("Invalid direction idx");
                }
                _direction = value;
                if (unitVectors[value].x == 0)
                {
                    width = 1;
                    height = length;
                }
                else
                {
                    width = length;
                    height = 1;
                }
            }
        }
        int _length;
        public int length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
                direction = _direction;
            }
        }
    }

    public readonly int width, height;
    public int startX, startY;

    public TileType[,] tileTypes;
    public List<Hallway> hallways;
    public List<Room> rooms;

    public Dungeon(int width, int height)
    {
        this.width = width;
        this.height = height;
        startX = -1;
        startY = -1;

        tileTypes = new TileType[width, height];
        hallways = new List<Hallway>();
        rooms = new List<Room>();
    }

    void GenerateRooms(GenerationOptions opts)
    {
        // Generate rooms
        int i;
        for (i = 0; i < opts["numRooms"]; i++)
        {
            int width = Random.Range(opts["roomWidthMin"], opts["roomWidthMax"] + 1),
                height = Random.Range(opts["roomHeightMin"], opts["roomHeightMax"] + 1);

            Room r = new Room();
            r.width = width;
            r.height = height;

            // Try 'placementTries' times to find a location that doesn't intersect
            // any existing rooms. If one is not found, fuck it, put it in the last
            // place we checked.
            int triesLeft = opts["placementTries"];
            while (triesLeft > 0)
            {
                // Generate candidate location
                r.x0 = Random.Range(0, width - r.width);
                r.y0 = Random.Range(0, height - r.height);

                // Check for intersection with other rooms
                if (!Collides(r))
                {
                    break;
                }
                triesLeft--;
            }

            // Add to the room list
            rooms.Add(r);
        }
    }

    void GenerateHallways(GenerationOptions opts)
    {
        Room startRoom = rooms[Random.Range(0, rooms.Count)];
        startX = Random.Range(startRoom.x0, startRoom.x0 + startRoom.width);
        startY = Random.Range(startRoom.y0, startRoom.y0 + startRoom.height);

    }


    Room RandomRoom(bool includeRooms = true)
    {
        if (!includeRooms)
        {
            return hallways[Random.Range(0, hallways.Count)];
        }
        int idx = Random.Range(0, rooms.Count + hallways.Count);
        if (idx > rooms.Count - 1)
        {
            return hallways[idx - rooms.Count];
        }
        return rooms[idx];
    }

    void GenerateTree(GenerationOptions opts)
    {
        // Create the starting room
        int srWidth = Random.Range(opts["roomWidthMin"], opts["roomWidthMax"]+1),
            srHeight = Random.Range(opts["roomHeightMin"], opts["roomHeightMax"]+1);
        Room startRoom = new Room(
            (width - srWidth) / 2, (height - srHeight) / 2,
            srWidth, srHeight
        );
        startX = Random.Range(startRoom.x0, startRoom.x1+1);
        startY = Random.Range(startRoom.y0, startRoom.y1+1);
        rooms.Add(startRoom);

        int candidatesTried = 0;
        int[] dirSuc = new int[4];

        // Generate requested number of rooms and corridors
        int hallwaysLeft = opts["numRooms"] - 1 + opts["numExtraCorridors"];
        int roomsLeft = opts["numRooms"];
        while (roomsLeft + hallwaysLeft > 0)
        {
            candidatesTried++;

            // Try a candidate branch
            int typeSelect = Random.Range(0, roomsLeft + hallwaysLeft);
            bool isHallway = typeSelect <= hallwaysLeft;
            if (hallways.Count == 0) isHallway = true;
            Room hostRoom = RandomRoom(includeRooms: isHallway);

            // Select a point on a wall from which to branch
            int ptX, ptY;
            int direction;
            int wallNum;
            if (!isHallway && hostRoom.GetType().IsSubclassOf(typeof(Hallway)))
            {
                if (Random.value > 0.5)
                {
                    wallNum = ((hostRoom as Hallway).direction + 2) % 4;
                }
                else
                {
                    wallNum = (hostRoom as Hallway).direction;
                }
            }
            else
            {
                wallNum = Random.Range(0, 4);
            }
            switch (wallNum)
            {
                case 0:
                    // Right wall
                    direction = 0;
                    ptX = hostRoom.x1 + 1;
                    ptY = Random.Range(hostRoom.y0, hostRoom.y1+1);
                    break;
                case 1:
                    // Top wall
                    direction = 1;
                    ptX = Random.Range(hostRoom.x0, hostRoom.x1+1);
                    ptY = hostRoom.y1 + 1;
                    break;
                case 2:
                    // Left wall
                    direction = 2;
                    ptX = hostRoom.x0 - 1;
                    ptY = Random.Range(hostRoom.y0, hostRoom.y1+1);
                    break;
                case 3:
                    // Bottom wall
                    direction = 3;
                    ptX = Random.Range(hostRoom.x0, hostRoom.x1+1);
                    ptY = hostRoom.y0 - 1;
                    break;
                default:
                    throw new KeyNotFoundException("Invalid direction idx");
            }

            if (isHallway)
            {
                // Try to generate a corridor
                int length = Random.Range(opts["corridorLengthMin"], opts["corridorLengthMax"]+1);
                Hallway candidate = new Hallway(ptX, ptY, direction, length);
                if (candidate.x0 > 0 && candidate.y0 > 0 &&
                    candidate.y0 + candidate.height < height &&
                    candidate.x0 + candidate.width < width && 
                    !Collides(candidate, checkHallways: true, checkRooms: true))
                {
                    hallways.Add(candidate);
                    dirSuc[direction]++;
                    hallwaysLeft--;
                }
            }
            else
            {
                // Try to generate a room
                int cWidth = Random.Range(opts["roomWidthMin"], opts["roomWidthMax"]+1),
                    cHeight = Random.Range(opts["roomHeightMin"], opts["roomHeightMax"]+1);
                // Calculate tangent to connection point
                int tX = Mathf.Abs(unitVectors[direction].y) > 0 ? -1 : 0,
                    tY = Mathf.Abs(unitVectors[direction].x) > 0 ? -1 : 0;
                ptX += tX * Random.Range(0, cWidth - 1);
                ptY += tY * Random.Range(0, cHeight - 1);
                if (unitVectors[direction].x < 0)
                {
                    ptX -= cWidth - 1;
                }
                if (unitVectors[direction].y < 0)
                {
                    ptY -= cHeight - 1;
                }

                Room candidate = new Room(ptX, ptY, cWidth, cHeight);
                if (candidate.x0 > 0 && candidate.y0 > 0 &&
                    candidate.x0 + candidate.width < width &&
                    candidate.y0 + candidate.height < height &&
                    !Collides(candidate, checkHallways: true, checkRooms: true))
                {
                    rooms.Add(candidate);
                    dirSuc[direction]++;
                    roomsLeft--;
                }
            }
        }
        Debug.Log(string.Format("Tried {0} candidate features, generated {1} rooms and {2} hallways", candidatesTried, rooms.Count, hallways.Count));
        Debug.Log(string.Format("Right: {0} Up: {1} Left: {2} Down: {3}", dirSuc[0], dirSuc[1], dirSuc[2], dirSuc[3]));
    }

    bool Collides(Room candidate, bool checkHallways=true, bool checkRooms=true)
    {
        if (checkHallways)
        {
            foreach (Room other in hallways)
            {
                if (other.Intersects(candidate))
                {
                    return true;
                }
            }
        }
        if (checkRooms)
        {
            foreach (Room other in rooms)
            {

                if (other.Intersects(candidate))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Regenerate(GenerationOptions opts)
    {
        // Empty rooms and hallways
        rooms.Clear();
        //hallways.Clear();

        // Clear out tile type array
        int i, j;
        for (j = 0; j < height; j++)
        {
            for (i = 0; i < width; i++)
            {
                tileTypes[i, j] = TileType.EMPTY;
            }
        }

        //GenerateRooms(opts);
        //GenerateHallways(opts);
        GenerateTree(opts);

        foreach (Room r in rooms.Union(hallways.Select((h) => (Room)h)))
        {
            for (i = r.x0 - 1; i <= r.x1 + 1; i++)
            {
                if (tileTypes[i, r.y0 - 1] == TileType.EMPTY)
                {
                    tileTypes[i, r.y0 - 1] = TileType.WALL;
                }
                if (tileTypes[i, r.y0 + r.height] == TileType.EMPTY)
                {
                    tileTypes[i, r.y0 + r.height] = TileType.WALL;
                }
            }
            for (j = r.y0 - 1; j <= r.y1 + 1; j++)
            {
                if (tileTypes[r.x0 - 1, j] == TileType.EMPTY)
                {
                    tileTypes[r.x0 - 1, j] = TileType.WALL;
                }
                if (tileTypes[r.x0 + r.width, j] == TileType.EMPTY)
                {
                    tileTypes[r.x0 + r.width, j] = TileType.WALL;
                }
            }
            TileType ft;
            if (r.width == 1 || r.height == 1)
            {
                ft = TileType.TUNNEL;
            }
            else
            {
                ft = TileType.FLOOR;
            }
            for (i = r.x0; i <= r.x1; i++)
            {
                for (j = r.y0; j <= r.y1; j++)
                {
                    tileTypes[i, j] = ft;
                }
            }
        }

        tileTypes[startX, startY] = TileType.START;
        
        Debug.Log(ToString());
    }

    public override string ToString()
    {
        string stringized = "";
        int i, j;
        for (j = height - 1; j >= 0; j--)
        {
            for (i = 0; i < width; i++)
            {
                switch (tileTypes[i, j])
                {
                    case TileType.EMPTY:
                        stringized += "/";
                        break;
                    case TileType.FLOOR:
                        stringized += " ";
                        break;
                    case TileType.TUNNEL:
                        stringized += "-";
                        break;
                    case TileType.WALL:
                        stringized += "x";
                        break;
                    case TileType.START:
                        stringized += "%";
                        break;
                    case TileType.ITEM:
                        stringized += "@";
                        break;
                    case TileType.QUEST:
                        stringized += "#";
                        break;
                    default:
                        stringized += "?";
                        break;
                }
            }
            stringized += "\n";
        }
        return stringized;
    }

    public static Vector2[] unitVectors = new Vector2[]
    {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)
    };
}
