using System.Security.Cryptography.X509Certificates;
using Domain.Model;

namespace Datasource.DTO;

public class FieldDto
{

    #region Properties
        public Int32 Id {get;set;}
        public Guid GameUUID {get;set;}
        public string MatrixJson {get;set;}
        
    #endregion Properties
    #region Constructors
    public FieldDto()
    {
        GameUUID=Guid.Empty;
        MatrixJson=string.Empty;
    }
    public FieldDto(Guid id,Guid gameUUId,Field field)
    {
        GameUUID = gameUUId;
        MatrixJson = ConvertMatrixToJson(field);
    }
    public FieldDto(Guid id,Game game)
    {
        GameUUID = game.UUID;
        MatrixJson = ConvertMatrixToJson(game.Field);
    }
    #endregion Constructors
    #region Methods
    #region  PrivateMethods
    private string ConvertMatrixToJson(Field field)
    {
        Dictionary<int,int[]> matrixDict = new Dictionary<int, int[]>(3);
        for(int i = 0; i < Field.MaxSize; i++)
        {
            int[] curentRow = new int[3];
            for(int j = 0; j < Field.MaxSize; j++)
            {
                curentRow[j] = field.Matrix[i,j];
            }
            matrixDict.Add(i,curentRow);
        }
        return System.Text.Json.JsonSerializer.Serialize(matrixDict);
    }
    public Field ConvertToField()
    {
        // Implement the logic to convert the JSON string back to a matrix
        // This is a placeholder implementation
        Dictionary<int,int[]> matrixDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<int,int[]>>(MatrixJson);
        int[,] matrix = new int[3,3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                matrix[i,j] = matrixDict[i][j];
            }
        }
        Field field = new Field();
        field.Matrix = matrix;
        return field;
    }
    #endregion PrivateMethods
    #endregion Methods 
}