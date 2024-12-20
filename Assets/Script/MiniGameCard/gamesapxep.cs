using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamesapxep : MonoBehaviour
{
    [SerializeField]
    private Transform gameTransform;

    [SerializeField]
    private Transform piecePrefab;

    private List<Transform> pieces;
    private int emptyLocation;
    private int size;
    private bool shuffling = false;
    public int completionCount = 0; // Biến đếm số lần hoàn thành

    public GameObject player;
    private Dichuyennv1 dichuyen1Script;

    public GameObject completionMessage;
    public GameObject openxephinh;
    public bool isGameCompleted = false; // Cờ để kiểm tra game đã hoàn thành hay chưa

    // Create the game setup with size x size pieces.
    private void CreateGamePieces(float gapThickness)
    {
        // This is the width of each tile.
        float width = 1 / (float)size;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameTransform);
                pieces.Add(piece);
                // Pieces will be in a game board going from -1 to +1.
                piece.localPosition = new Vector3(
                    -1 + (2 * width * col) + width,
                    +1 - (2 * width * row) - width,
                    0
                );
                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";
                // We want an empty space in the bottom right.
                if ((row == size - 1) && (col == size - 1))
                {
                    emptyLocation = (size * size) - 1;
                    piece.gameObject.SetActive(false);
                }
                else
                {
                    // We want to map the UV coordinates appropriately, they are 0->1.
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    if (mesh.vertexCount == 4)
                    { // Ensure that the mesh has 4 vertices (a quad)
                        Vector2[] uv = new Vector2[4];
                        // UV coord order: (0, 1), (1, 1), (0, 0), (1, 0)
                        uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
                        uv[1] = new Vector2(
                            (width * (col + 1)) - gap,
                            1 - ((width * (row + 1)) - gap)
                        );
                        uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
                        uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));
                        // Assign our new UVs to the mesh.
                        mesh.uv = uv;
                    }
                    else
                    {
                        Debug.LogWarning("Mesh has an unexpected number of vertices.");
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        dichuyen1Script = player.GetComponent<Dichuyennv1>();
        completionMessage.SetActive(false); // Đảm bảo panel thông báo bị tắt lúc đầu
        pieces = new List<Transform>();
        size = 3;
        isGameCompleted = false; // Đặt lại cờ là chưa hoàn thành khi bắt đầu game
        completionCount = 0; // Đặt lại biến đếm số lần hoàn thành
        CreateGamePieces(0.01f);
    }

    private IEnumerator HideCompletionMessage()
    {
        yield return new WaitForSeconds(3f);
        if (completionMessage != null)
        {
            completionMessage.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(completionCount);
        // Check for completion.
        if (!shuffling && CheckCompletion())
        {
            shuffling = true;
            StartCoroutine(WaitShuffle(0.5f));
        }

        // On click send out ray to see if we click a piece.
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero
            );
            if (hit)
            {
                // Go through the list, the index tells us the position.
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        // Check each direction to see if valid move.
                        // We break out on success so we don't carry on and swap back again.
                        if (SwapIfValid(i, -size, size))
                        {
                            break;
                        }
                        if (SwapIfValid(i, +size, size))
                        {
                            break;
                        }
                        if (SwapIfValid(i, -1, 0))
                        {
                            break;
                        }
                        if (SwapIfValid(i, +1, size - 1))
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    // colCheck is used to stop horizontal moves wrapping.
    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        if (((i % size) != colCheck) && ((i + offset) == emptyLocation))
        {
            // Swap them in game state.
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);
            // Swap their transforms.
            (pieces[i].localPosition, pieces[i + offset].localPosition) = (
                (pieces[i + offset].localPosition, pieces[i].localPosition)
            );
            // Update empty location.
            emptyLocation = i;
            return true;
        }
        return false;
    }

    // We name the pieces in order so we can use this to check completion.
    private bool CheckCompletion()
    {
        // Kiểm tra nếu các mảnh ghép đã được sắp xếp đúng
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name != $"{i}")
            {
                return false; // Nếu mảnh ghép chưa đúng vị trí, không hoàn thành
            }
        }

        // Kiểm tra số lần hoàn thành
        if (completionCount == 1) // Nếu đây là lần hoàn thành thứ 2
        {
            if (!isGameCompleted)
            {
                isGameCompleted = true;
                openxephinh.SetActive(false); // Tắt các vật phẩm hoặc giao diện không cần thiết
                if (completionMessage != null)
                {
                    completionMessage.SetActive(true); // Hiển thị thông báo hoàn thành
                    StartCoroutine(HideCompletionMessage()); // Tắt thông báo sau 3 giây
                }
            }
            if (dichuyen1Script != null)
            {
                dichuyen1Script.enabled = true;
            }
            completionCount++;
        }
        else
        {
            // Nếu là lần hoàn thành đầu tiên, chỉ tăng số lần hoàn thành mà không hiển thị thông báo
            completionCount++;
        }

        return true; // Nếu tất cả các mảnh ghép đúng, trò chơi hoàn thành
    }

    private IEnumerator WaitShuffle(float duration)
    {
        yield return new WaitForSeconds(duration);
        Shuffle();
        shuffling = false;
    }

    // Brute force shuffling.
    private void Shuffle()
    {
        int count = 0;
        int last = 0;
        while (count < (size * size * size))
        {
            // Pick a random location.
            int rnd = Random.Range(0, size * size);
            // Only thing we forbid is undoing the last move.
            if (rnd == last)
            {
                continue;
            }
            last = emptyLocation;
            // Try surrounding spaces looking for valid move.
            if (SwapIfValid(rnd, -size, size))
            {
                count++;
            }
            else if (SwapIfValid(rnd, +size, size))
            {
                count++;
            }
            else if (SwapIfValid(rnd, -1, 0))
            {
                count++;
            }
            else if (SwapIfValid(rnd, +1, size - 1))
            {
                count++;
            }
        }
    }
}
