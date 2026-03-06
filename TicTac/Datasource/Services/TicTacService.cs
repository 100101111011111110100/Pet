using Domain.Interfaces;
using Domain.Model;
using System.Numerics;
using System.Reflection;
using Datasource.Iinterfaces;

namespace Datasource.Services;

public class TicTacService:ITicTacService
{
    #region Properties
        private IGameRepository _repo { get; set; }
    #endregion Properties
    #region Constructors
    public TicTacService(IGameRepository repo)=>_repo = repo;
    #endregion Constructors
    #region Methods
    #region PublicMethods

    /// <summary>
    /// Производит следующий ход алгоритмом МинМакс
    /// </summary>
    /// <param name="field">Поле на котором нужно произвести следующий ход</param>
    public async Task NextTurn(Guid gameUUID)
    {
        var game = _repo.GetGame(gameUUID);
        Field field = game.Field.Clone();
        var nextMove = await MinMaxAlg(field);
        field.Matrix[nextMove.r, nextMove.c] = UserSymbols.AISymbol;
        game.Field.Matrix = field.Matrix;
        _repo.SetGame(game);
    }

    /// <summary>
    /// Проводит валидацию игрового поля пользователя,
    /// относительно текущего игрового поля
    /// </summary>
    /// <param name="gameField">текущее игровое поле игры</param>
    /// <param name="userField">текущее игровое поле игрока</param>
    /// <returns>
    /// Истина - если поле является валидным
    /// Ложь   - если поле не является валидным
    /// Валидность :
    /// Пользователь произвёл только один ход
    /// Пользователь сделал ход за свою сторону
    /// </returns>
    public bool ValidationField(Game game, User user) => MainValidationLogic(game.Field, user.Field,user.Symbol);

    /// <summary>
    /// Проверяет закончена ли игра
    /// </summary>
    /// <param name="field">Передаём игровое поле</param>
    /// <returns>
    /// Истина -- если игра окончена
    /// Ложь   -- если игра не окончена
    /// </returns>
    public bool IsGameEnded(Field field)
    {
        bool isFieldHaveEmptyElem = field.EmptyCountCells>0;
        if (isFieldHaveEmptyElem && CheckWinCondition(field).Equals(Player.None))
        {
            return false;
        }
            return true;
    }
    public (bool, Guid) IsGameEnded(Game game) => game.State switch
    {
        State.PlayersXVictory => (true,game.XPlayer),
        State.PlayersOVictory => (true, game.OPlayer),
        State.Draw => (true, Guid.Empty),
        _=>(false,Guid.Empty)
    };
    public Player WhoIsWinner(Field field) => CheckWinCondition(field);
    #endregion PublicMethods
    #region PrivateMethods
    private bool MainValidationLogic(Field gameField, Field userField,int userSymbol)
    {

        byte count = 0;
        bool emptyMove = true;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (gameField[i, j] != 0 && userField[i, j] != gameField[i, j]) //Проверка что пользователь не изменил предыдущее значение переменной
                {
                    return false;
                }
                if (gameField[i, j] == 0 && userField[i, j] != 0) //Поиск поля в которое пользователь поставил свой ход
                {
                    if (++count > 1)// Проверка пользователь сделал больше одного хода
                    {
                        return false;
                    }
                    if (userField[i, j] != userSymbol) //Проверка пользователь поставил свой символ
                    {
                        return false;
                    }
                }
                emptyMove = emptyMove && gameField[i, j] == userField[i, j];//Проврека пользователь отправил пустое поле
            }
        }
        //Если поле пустое то должны вернуть false(значит поле не валидно) иначе правду(значит пользователь сделал только один ход своим символом)
        return emptyMove ? !emptyMove : true;
    }
    private async Task<(int r, int c)> MinMaxAlg(Field field)
    {
        List<(Task<(int weidth, int depth)> task, int row, int col)> tasks = new List<(Task<(int weidth, int depth)> task, int row, int col)>(9);
        for (byte i = 0; i < 3; i++)
        {
            for (byte j = 0; j < 3; j++)
            {
                if (field.Matrix[i, j].Equals(0))
                {
                    byte row = i;
                    byte col = j;
                    tasks.Add((Task<(int weidth, int depth)>.Run(() => RecursiveLogic(field.Clone(), row, col, Player.Ai)), row, col));
                }
            }
        }
        await Task.WhenAll(tasks.Select(t => t.task).ToList());
        var r = tasks.OrderByDescending(t => t.task.Result.weidth).ThenBy(t => t.task.Result.depth).First();
        return (r.row, r.col);


        //recurent method here
        (int weidth, int depth) RecursiveLogic(Field field, byte r, byte c, Player currentPlayer, int depth = 0)
        {
            field.Matrix[r, c] = currentPlayer.Equals(Player.Ai) ? UserSymbols.AISymbol : UserSymbols.PlayerSymbol;
            if (MinMaxVinCondition(field))
            {
                return CheckWinCondition(field) switch
                {
                    Player.Ai => (10, 0),
                    Player.User => (-10, 0),
                    _ => (0, depth)
                };
            }
            else if (NonUserWin(field)) //проверка на ничью в случае заполненности поляи и отсутствия победителя
            {
                return (0, depth);
            }
            int weidth = 0;
            int tempDepth = int.MaxValue;
            if (currentPlayer.Equals(Player.Ai))//Max
            {
                weidth = int.MinValue;
                for (byte i = 0; i < 3; i++)
                {
                    for (byte j = 0; j < 3; j++)
                    {
                        if (field.Matrix[i, j].Equals(0))
                        {
                            (int calcedWeidth, int calcedDepth) = RecursiveLogic(field, i, j, Player.User, depth + 1);
                            if (calcedWeidth > weidth || (calcedWeidth == weidth && calcedDepth < tempDepth))
                            {
                                weidth = calcedWeidth;
                                tempDepth = calcedDepth;
                            }
                            // weidth = calcedWeidth > weidth ? calcedWeidth : weidth;
                            field.Matrix[i, j] = 0;
                        }
                    }
                }
            }
            else//Min
            {
                weidth = int.MaxValue;
                for (byte i = 0; i < 3; i++)
                {
                    for (byte j = 0; j < 3; j++)
                    {
                        if (field.Matrix[i, j].Equals(0))
                        {
                            (int calcedWeidth, int calcedDepth) = RecursiveLogic(field, i, j, Player.Ai, depth + 1);
                            if (calcedWeidth < weidth || (calcedWeidth == weidth && calcedDepth < tempDepth))
                            {
                                weidth = calcedWeidth;
                                tempDepth = calcedDepth;
                            }
                            // weidth = calcedWeidth < weidth ? calcedWeidth : weidth;
                            field.Matrix[i, j] = 0;
                        }
                    }
                }
            }
            return (weidth, tempDepth + 1);
        }
        bool MinMaxVinCondition(Field field) => CheckHorizontalWinCondition(field).Item1 || CheckVerticalWinCondition(field).Item1 || CheckDiagonalWinCondition(field).Item1;
        bool NonUserWin(Field field)
        {
            foreach (var r in field.Matrix)
            {
                if (r.Equals(0)) return false;
            }
            return true;
        }

    }

    internal Player CheckWinCondition(Field field)
    {
        var horizontalCortage = CheckHorizontalWinCondition(field);
        if (horizontalCortage.Item1)
        {
            return field.Matrix[horizontalCortage.row, horizontalCortage.col] == UserSymbols.PlayerSymbol ? Player.User : Player.Ai;
        }
        var verticalCortage = CheckVerticalWinCondition(field);
        if (verticalCortage.Item1)
        {
            return field.Matrix[verticalCortage.row, verticalCortage.col] == UserSymbols.PlayerSymbol ? Player.User : Player.Ai;
        }
        var diagonalCortage = CheckDiagonalWinCondition(field);
        if (diagonalCortage.Item1)
        {
            return field.Matrix[diagonalCortage.row, diagonalCortage.col] == UserSymbols.PlayerSymbol ? Player.User : Player.Ai;
        }
        return Player.None;
    }

    private (bool, int row, int col) CheckHorizontalWinCondition(Field field)
    {
        bool res = false;
        int row, col;
        col = -1;
        for (row = 0; !res && row < 3; row++)
        {
            res = field.Matrix[row, 0] == field.Matrix[row, 1] && field.Matrix[row, 1] == field.Matrix[row, 2] && field.Matrix[row, 0] != 0 && field.Matrix[row, 1] != 0 && field.Matrix[row, 2] != 0;
            if (res)
            {
                col = 0;
            }
        }
        return (res, row - 1, col);
    }

    private (bool, int row, int col) CheckVerticalWinCondition(Field field)
    {
        bool res = false;
        int row, col;
        row = -1;
        for (col = 0; !res && col < 3; col++)
        {
            res = field.Matrix[0, col] == field.Matrix[1, col] && field.Matrix[1, col] == field.Matrix[2, col] && field.Matrix[0, col] != 0 && field.Matrix[1, col] != 0 && field.Matrix[2, col] != 0;
            if (res)
            {
                row = 0;
            }
        }
        return (res, row, col - 1);
    }
    private (bool, int row, int col) CheckDiagonalWinCondition(Field field)
    {
        bool res = (field.Matrix[0, 0] == field.Matrix[1, 1] && field.Matrix[1, 1] == field.Matrix[2, 2] && field.Matrix[0, 0] != 0 && field.Matrix[1, 1] != 0 && field.Matrix[2, 2] != 0) ||
        (field.Matrix[0, 2] == field.Matrix[1, 1] && field.Matrix[1, 1] == field.Matrix[2, 0] && field.Matrix[0, 2] != 0 && field.Matrix[1, 1] != 0 && field.Matrix[2, 0] != 0);
        return (res, 1, 1);
    }
    
    #endregion PrivateMethods
    #endregion Methods
}
