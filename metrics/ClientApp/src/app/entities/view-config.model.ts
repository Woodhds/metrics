export class ViewConfig {
  public name: string;
  public lookupProperty: string;
  public columns: ColumnView[] = [];
}

export class ColumnView {
  public name: string;
  public title: string;

}
