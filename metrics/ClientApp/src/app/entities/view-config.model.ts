export class ViewConfig {
  public Name: string;
  public LookupProperty: string;
  public Columns: ColumnView[] = [];
}

export class ColumnView {
  public Name: string;
  public Title: string;

}
