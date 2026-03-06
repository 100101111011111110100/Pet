using Domain.Model;

namespace WebApi.DTO;

public class FieldDto
{
    // public char [,] Field {get;} = new char [3,3] {{UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol},
    //                             {UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol},
    //                             {UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol}};
    public Dictionary<byte, char[]> Field { get; set; } = new Dictionary<byte, char[]>()
    {
        {0, new char[3]{UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol}},
        {1, new char[3]{UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol}},
        {2, new char[3]{UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol,UserSymbols.EmptyCharSymbol}}
    };

    public FieldDto() { }
    public FieldDto(Field field)
    {
        ConvertFromDomainField(field);
    }
    public void ConvertFromDomainField(Field field)
    {
        for (byte i = 0; i < 3; i++)
        {
            for (byte j = 0; j < 3; j++)
            {
                Field[i][j] = UserSymbols.EmptyCharSymbol;
            }
        }
        for (byte i = 0; i < 3; i++)
        {
            for (byte j = 0; j < 3; j++)
            {
                if (field.Matrix[i, j] == 0)
                    continue;
                Field[i][j] = field.Matrix[i, j].Equals(UserSymbols.AISymbol) ?
                    UserSymbols.AICharSymbol :
                    UserSymbols.PlayerCharSymbol;
            }
        }
    }
    public Field ConvertToDomainField()
    {
        Field field = new Field();
        for (byte i = 0; i < 3; i++)
        {
            for (byte j = 0; j < 3; j++)
            {
                if (Field[i][j] == UserSymbols.EmptyCharSymbol)
                    continue;
                field.Matrix[i, j] = Field[i][j].Equals(UserSymbols.AICharSymbol) ?
                    UserSymbols.AISymbol :
                    UserSymbols.PlayerSymbol;
            }
        }
        return field;
    }
}