class Board:
    def __init__(self):
        self.grid = [[" "] * 3 for _ in range(3)]

    def display(self, highlight=None):
        highlight = highlight or set()
        print("  1 2 3")
        for r, row in enumerate(self.grid):
            line = []
            for c, cell in enumerate(row):
                if (r, c) in highlight:
                    line.append("-")
                else:
                    line.append(cell)
            print(r + 1, "|".join(line))
            if r < 2:
                print("  -----")

    def make_move(self, row, col, symbol):
        if self.grid[row][col] == " ":
            self.grid[row][col] = symbol
            return True
        return False

    def is_full(self):
        return all(cell != " " for row in self.grid for cell in row)

    def get_winning_line(self, symbol):
        for r in range(3):
            if all(self.grid[r][c] == symbol for c in range(3)):
                return {(r, c) for c in range(3)}

        for c in range(3):
            if all(self.grid[r][c] == symbol for r in range(3)):
                return {(r, c) for r in range(3)}

        if all(self.grid[i][i] == symbol for i in range(3)):
            return {(i, i) for i in range(3)}

        if all(self.grid[i][2 - i] == symbol for i in range(3)):
            return {(i, 2 - i) for i in range(3)}

        return None

    def check_winner(self, symbol):
        lines = []
        lines.extend(self.grid)
        lines.extend([[self.grid[r][c] for r in range(3)] for c in range(3)])
        lines.append([self.grid[i][i] for i in range(3)])
        lines.append([self.grid[i][2 - i] for i in range(3)])
        return any(all(cell == symbol for cell in line) for line in lines)


class Player:
    def __init__(self, name, symbol):
        self.name = name
        self.symbol = symbol

    def get_move(self):
        while True:
            move = input(f"{self.name} ({self.symbol}), podaj ruch jako wiersz i kolumna (np. 2 3): ")
            parts = move.split()
            if len(parts) != 2:
                print("Podaj dwie liczby oddzielone spacją.")
                continue
            if not parts[0].isdigit() or not parts[1].isdigit():
                print("Wprowadź liczby.")
                continue
            row, col = int(parts[0]), int(parts[1])
            if row < 1 or row > 3 or col < 1 or col > 3:
                print("Wiersz i kolumna muszą być w zakresie 1-3.")
                continue
            return row - 1, col - 1


class Game:
    def __init__(self):
        self.board = Board()
        self.player1 = Player("Gracz 1", "X")
        self.player2 = Player("Gracz 2", "O")
        self.current_player = self.player1

    def switch_player(self):
        self.current_player = self.player1 if self.current_player == self.player2 else self.player2

    def ask_replay(self):
        while True:
            answer = input("Czy chcesz rozpocząć nową grę? (t/n): ").strip().lower()
            if answer in ("t", "tak"):
                return True
            if answer in ("n", "nie"):
                return False
            print("Wpisz 't' lub 'n'.")

    def play_round(self):
        while True:
            self.board.display()
            if self.board.is_full():
                print("Remis!")
                break
            row, col = self.current_player.get_move()
            if not self.board.make_move(row, col, self.current_player.symbol):
                print("To pole jest zajęte. Spróbuj jeszcze raz.")
                continue

            winning_line = self.board.get_winning_line(self.current_player.symbol)
            if winning_line is not None:
                self.board.display(highlight=winning_line)
                print(f"{self.current_player.name} wygrał!")
                break

            if self.board.is_full():
                self.board.display()
                print("Remis!")
                break
            self.switch_player()

    def run(self):
        while True:
            self.play_round()
            if not self.ask_replay():
                break
            self.board = Board()
            self.current_player = self.player1


if __name__ == "__main__":
    game = Game()
    game.run()
