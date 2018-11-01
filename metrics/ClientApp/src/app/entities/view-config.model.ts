export class ViewConfig {
  public Name: string;
  public LookupProperty: string;
  public Columns: ColumnView[] = [];
}

export class ColumnView {
  public Name: string;
  public Title: string;
  public Required: boolean;
  public Type: PropertyDataType;
}

export enum PropertyDataType {
  Integer = 0,
  String = 1
}
