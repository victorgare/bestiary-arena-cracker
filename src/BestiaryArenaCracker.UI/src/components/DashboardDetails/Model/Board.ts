export type Board = {
  tile: number;
  monster: {
    name: string;
    hp: number;
    ad: number;
    ap: number;
    armor: number;
    magicResist: number;
    level: number;
  };
  equipment: {
    name: string;
    stat: string;
    tier: number;
  };
};
