const DAY_PATTERN = /^\d{4}-\d{2}-\d{2}$/

export function parseDay(value: string | null): string {
  if (!value || !DAY_PATTERN.test(value)) return ''
  const [year, month, day] = value.split('-').map(Number)
  const date = new Date(year, month - 1, day)
  const valid = date.getFullYear() === year && date.getMonth() === month - 1 && date.getDate() === day
  return valid ? value : ''
}

export function startOfDayIso(day: string): string {
  const [year, month, date] = day.split('-').map(Number)
  return new Date(year, month - 1, date).toISOString()
}

export function endOfDayIso(day: string): string {
  const [year, month, date] = day.split('-').map(Number)
  return new Date(year, month - 1, date, 23, 59, 59, 999).toISOString()
}

export function formatStart(value: string): string {
  return new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value))
}
